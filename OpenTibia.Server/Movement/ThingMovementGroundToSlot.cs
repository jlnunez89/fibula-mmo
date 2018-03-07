using System;
using System.Linq;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;
using OpenTibia.Server.Events;
using OpenTibia.Server.Movement.Policies;
using OpenTibia.Server.Notifications;

namespace OpenTibia.Server.Movement
{
    internal class ThingMovementGroundToSlot : MovementBase
    {
        public Location FromLocation { get; }
        public byte FromStackPos { get; }
        public ITile FromTile { get; }

        public Location ToLocation { get; }
        public byte ToSlot { get; }

        public IThing Thing { get; }
        public byte Count { get; }

        public ThingMovementGroundToSlot(uint creatureRequestingId, IThing thingMoving, Location fromLocation, byte fromStackPos, Location toLocation, byte count = 1)
            : base(creatureRequestingId)
        {
            // intentionally left thing null check out. Handled by Perform().
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (count == 0 || count > 100)
            {
                throw new ArgumentException($"Invalid count {count}.", nameof(count));
            }
            
            FromLocation = fromLocation;
            FromStackPos = fromStackPos;
            FromTile = Game.Instance.GetTileAt(FromLocation);

            ToLocation = toLocation;
            ToSlot = (byte)toLocation.Slot;

            Thing = thingMoving;
            Count = count;

            var droppingItem = requestor?.Inventory?[ToSlot];

            Policies.Add(new SlotHasContainerAndContainerHasEnoughCapacityPolicy(RequestorId, droppingItem));
            Policies.Add(new GrabberHasEnoughCarryStrengthPolicy(RequestorId, Thing, droppingItem));
            Policies.Add(new ThingIsTakeablePolicy(RequestorId, Thing));
            Policies.Add(new LocationsMatchPolicy(Thing?.Location ?? new Location(), FromLocation));
            Policies.Add(new TileContainsThingPolicy(Thing, FromLocation, Count));
        }

        public override void Perform()
        {
            var updatedItem = Thing as IItem;
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (FromTile == null || Thing == null || updatedItem == null || requestor == null)
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

            // and call any separation events.
            if (FromTile.HandlesSeparation) // TODO: what happens on separation of less than required quantity, etc?
            {
                foreach (var itemWithSeparation in FromTile.ItemsWithSeparation)
                {
                    var separationEvents = Game.Instance.EventsCatalog[EventType.Separation].Cast<SeparationEvent>();

                    var candidate = separationEvents.FirstOrDefault(e => e.ThingIdOfSeparation == itemWithSeparation.Type.TypeId && e.Setup(itemWithSeparation, requestor) && e.CanBeExecuted);

                    // Execute all actions.
                    candidate?.Execute();
                }
            }

            if (thing != Thing)
            {
                // item got split cause we removed less than the total amount.
                // update the thing we're adding to the container.
                updatedItem = thing as IItem;
            }

            if (updatedItem == null)
            {
                return;
            }

            // attempt to place the intended item at the slot.
            IItem addedItem;
            if (!requestor.Inventory.Add(updatedItem, out addedItem, ToSlot, updatedItem.Count))
            {
                // failed to add to the slot, add again to the source tile
                FromTile.AddThing(ref thing, thing.Count);

                // notify all spectator players of that tile.
                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, FromTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, FromTile.Location)), FromTile.Location);

                // call any collision events again.
                if (FromTile.HandlesCollision)
                {
                    foreach (var itemWithCollision in FromTile.ItemsWithCollision)
                    {
                        var collisionEvents = Game.Instance.EventsCatalog[EventType.Collision].Cast<CollisionEvent>();

                        var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, updatedItem) && e.CanBeExecuted);

                        // Execute all actions.
                        candidate?.Execute();
                    }
                }
            }
            else
            {
                // added the new item to the slot
                if (addedItem == null)
                {
                    return;
                }

                // we exchanged or got some leftover item, place back in the source container at any index.
                IThing remainderThing = addedItem;

                FromTile.AddThing(ref remainderThing, remainderThing.Count);

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

                    var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, remainderThing) && e.CanBeExecuted);

                    // Execute all actions.
                    candidate?.Execute();
                }
            }
        }
    }
}