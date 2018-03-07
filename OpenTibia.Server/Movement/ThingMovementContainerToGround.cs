using System;
using System.Linq;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;
using OpenTibia.Server.Events;
using OpenTibia.Server.Movement.Policies;
using OpenTibia.Server.Notifications;

namespace OpenTibia.Server.Movement
{
    internal class ThingMovementContainerToGround : MovementBase
    {
        public Location FromLocation { get; }
        public IContainer FromContainer { get; }
        public byte FromIndex { get; }

        public Location ToLocation { get; }
        public ITile ToTile { get; }

        public IThing Thing { get; }
        public byte Count { get; }

        public ThingMovementContainerToGround(uint creatureRequestingId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base (creatureRequestingId)
        {
            // intentionally left thing null check out. Handled by Perform().
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            if (requestor == null)
            {
                throw new ArgumentNullException(nameof(requestor));
            }

            Thing = thingMoving;
            Count = count;

            FromLocation = fromLocation;
            FromContainer = (requestor as IPlayer)?.GetContainer(FromLocation.Container);
            FromIndex = (byte)FromLocation.Z;

            ToLocation = toLocation;
            ToTile = Game.Instance.GetTileAt(ToLocation);

            Policies.Add(new CanThrowBetweenPolicy(RequestorId, requestor.Location, ToLocation));
            Policies.Add(new GrabberHasContainerOpenPolicy(RequestorId, FromContainer));
            Policies.Add(new ContainerHasItemAndEnoughAmountPolicy(Thing as IItem, FromContainer, FromIndex, Count));
            Policies.Add(new LocationNotObstructedPolicy(RequestorId, Thing, ToLocation));
            Policies.Add(new LocationHasTileWithGroundPolicy(ToLocation));
        }

        public override void Perform()
        {
            IItem extraItem;
            var itemToUpdate = Thing as IItem;
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (FromContainer == null || ToTile == null || itemToUpdate == null)
            {
                return;
            }

            // attempt to remove from the source container
            if (!FromContainer.RemoveContent(itemToUpdate.Type.TypeId, FromIndex, Count, out extraItem))
            {
                return;
            }

            if (extraItem != null)
            {
                itemToUpdate = extraItem;
            }

            // add the remaining item to the destination tile.
            IThing thing = itemToUpdate;
            ToTile.AddThing(ref thing, itemToUpdate.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, ToTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, ToTile.Location)), ToTile.Location);

            // and handle collision.
            if (!ToTile.HandlesCollision)
            {
                return;
            }

            foreach (var itemWithCollision in ToTile.ItemsWithCollision)
            {
                var collisionEvents = Game.Instance.EventsCatalog[EventType.Collision].Cast<CollisionEvent>();

                var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, thing, requestor as IPlayer) && e.CanBeExecuted);

                // Execute all actions.
                candidate?.Execute();
            }
        }
    }
}