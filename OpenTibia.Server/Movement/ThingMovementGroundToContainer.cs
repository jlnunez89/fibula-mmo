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
    internal class ThingMovementGroundToContainer : MovementBase
    {
        public Location FromLocation { get; }
        public byte FromStackPos { get; }
        public ITile FromTile { get; }

        public Location ToLocation { get; }
        public IContainer ToContainer { get; }
        public byte ToIndex { get; }

        public IThing Thing { get; }
        public byte Count { get; }

        public ThingMovementGroundToContainer(uint creatureRequestingId, IThing thingMoving, Location fromLocation, byte fromStackPos, Location toLocation, byte count = 1)
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
            FromStackPos = fromStackPos;
            FromTile = Game.Instance.GetTileAt(FromLocation);

            ToLocation = toLocation;
            ToContainer = (requestor as IPlayer)?.GetContainer(toLocation.Container);
            ToIndex = (byte)ToLocation.Z;

            if (ToContainer != null && ToContainer.HolderId == RequestorId)
            {
                Policies.Add(new GrabberHasEnoughCarryStrengthPolicy(RequestorId, Thing));
            }

            Policies.Add(new GrabberHasContainerOpenPolicy(RequestorId, ToContainer));
            Policies.Add(new ContainerHasEnoughCapacityPolicy(ToContainer));
            Policies.Add(new ThingIsTakeablePolicy(RequestorId, Thing));
            Policies.Add(new LocationsMatchPolicy(Thing?.Location ?? new Location(), FromLocation));
            Policies.Add(new TileContainsThingPolicy(Thing, FromLocation, Count));
        }

        public override void Perform()
        {
            var thingAsItem = Thing as IItem;
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (FromTile == null || ToContainer == null || Thing == null || thingAsItem == null)
            {
                return;
            }

            var thingAtTile = FromTile.GetThingAtStackPosition(FromStackPos);

            if (thingAtTile == null)
            {
                return;
            }
            
            var thing = Thing;
            FromTile.RemoveThing(ref thing, Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, FromTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, FromTile.Location)), FromTile.Location);
                        
            if (thing != Thing)
            {
                // item got split cause we removed less than the total amount.
                // update the thing we're adding to the container.
                thingAsItem = thing as IItem;
            }

            // attempt to add the item to the container.
            if (thingAsItem == null || ToContainer.AddContent(thingAsItem, ToIndex))
            {
                // and call any separation events.
                if (FromTile.HandlesSeparation) // TODO: what happens on separation of less than required quantity, etc?
                {
                    foreach (var itemWithSeparation in FromTile.ItemsWithSeparation)
                    {
                        var separationEvents = Game.Instance.EventsCatalog[EventType.Separation].Cast<SeparationEvent>();

                        var candidate = separationEvents.FirstOrDefault(e => e.ThingIdOfSeparation == itemWithSeparation.Type.TypeId && e.Setup(itemWithSeparation, thing, requestor as IPlayer) && e.CanBeExecuted);

                        // Execute all actions.
                        candidate?.Execute();
                    }
                }

                return;
            }
            
            // failed to add to the dest container (whole or partial)
            // add again to the source tile
            IThing itemAsThing = thingAsItem;
            FromTile.AddThing(ref itemAsThing, thingAsItem.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, FromTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, FromTile.Location)), FromTile.Location);
                
            // call any collision events again.
            if (!FromTile.HandlesCollision)
            {
                return;
            }

            foreach (var itemWithCollision in FromTile.ItemsWithCollision)
            {
                var collisionEvents = Game.Instance.EventsCatalog[EventType.Collision].Cast<CollisionEvent>();

                var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, Thing) && e.CanBeExecuted);

                // Execute all actions.
                candidate?.Execute();
            }
        }
    }
}