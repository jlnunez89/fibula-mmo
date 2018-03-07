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
    internal class ThingMovementContainerToContainer : MovementBase
    {
        public Location FromLocation { get; }
        public IContainer FromContainer { get; }
        public byte FromIndex { get; }

        public Location ToLocation { get; }
        public IContainer ToContainer { get; }
        public byte ToIndex { get; }

        public IThing Thing { get; }
        public byte Count { get; }

        public ThingMovementContainerToContainer(uint creatureRequestingId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base (creatureRequestingId)
        {
            // intentionally left thing null check out. Handled by Perform().
            var requestor = Game.Instance.GetCreatureWithId(RequestorId);

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
            ToContainer = (requestor as IPlayer)?.GetContainer(ToLocation.Container);
            ToIndex = (byte)ToLocation.Z;

            if (FromContainer?.HolderId != ToContainer?.HolderId && ToContainer?.HolderId == RequestorId)
            {
                Policies.Add(new GrabberHasEnoughCarryStrengthPolicy(RequestorId, Thing));
            }

            Policies.Add(new GrabberHasContainerOpenPolicy(RequestorId, FromContainer));
            Policies.Add(new ContainerHasItemAndEnoughAmountPolicy(Thing as IItem, FromContainer, FromIndex, Count));
            Policies.Add(new GrabberHasContainerOpenPolicy(RequestorId, ToContainer));
        }

        public override void Perform()
        {
            IItem extraItem;
            var updatedItem = Thing as IItem;
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (FromContainer == null || ToContainer == null || updatedItem == null || requestor == null) 
            {
                return;
            }
            
            // attempt to remove from the source container
            if (!FromContainer.RemoveContent(updatedItem.Type.TypeId, FromIndex, Count, out extraItem))
            {
                return;
            }

            if (extraItem != null)
            {
                updatedItem = extraItem;
            }

            // successfully removed thing from the source container
            // attempt to add to the dest container
            if (ToContainer.AddContent(updatedItem, ToIndex))
            {
                return;
            }

            // failed to add to the dest container (whole or partial)
            // attempt to add again to the source at any index.
            if (FromContainer.AddContent(updatedItem, 0xFF))
            {
                return;
            }

            // and we somehow failed to re-add it to the source container...
            // throw to the ground.
            IThing thing = updatedItem;
            requestor.Tile.AddThing(ref thing, updatedItem.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, requestor.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, requestor.Location)), requestor.Location);

            // and handle collision.
            if (!requestor.Tile.HandlesCollision)
            {
                return;
            }

            foreach (var itemWithCollision in requestor.Tile.ItemsWithCollision)
            {
                var collisionEvents = Game.Instance.EventsCatalog[EventType.Collision].Cast<CollisionEvent>();

                var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, thing) && e.CanBeExecuted);

                // Execute all actions.
                candidate?.Execute();
            }
        }
    }
}