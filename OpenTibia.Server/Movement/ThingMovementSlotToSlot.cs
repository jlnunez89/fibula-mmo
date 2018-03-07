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
    internal class ThingMovementSlotToSlot : MovementBase
    {
        public Location FromLocation { get; }
        public byte FromSlot { get; }

        public Location ToLocation { get; }
        public byte ToSlot { get; }

        public IItem Item { get; }
        public byte Count { get; }

        public ThingMovementSlotToSlot(uint creatureRequestingId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base (creatureRequestingId)
        {
            // intentionally left thing null check out. Handled by Perform().

            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            FromLocation = fromLocation;
            FromSlot = (byte)FromLocation.Slot;

            ToLocation = toLocation;
            ToSlot = (byte)ToLocation.Slot;

            Item = thingMoving as IItem;
            Count = count;
            
            Policies.Add(new SlotContainsItemAndCountPolicy(creatureRequestingId, Item, FromSlot, Count));
        }

        public override void Perform()
        {
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (Item == null || requestor == null)
            {
                return;
            }

            bool partialRemove;
            // attempt to remove the item from the inventory
            var movingItem = requestor.Inventory?.Remove(FromSlot, Count, out partialRemove);

            if (movingItem == null)
            {
                return;
            }

            // attempt to place the intended item at the slot.
            IItem addedItem;
            if (!requestor.Inventory.Add(movingItem, out addedItem, ToSlot, movingItem.Count))
            {
                // failed to add to the slot, add again to the source slot
                if (!requestor.Inventory.Add(movingItem, out addedItem, FromSlot, movingItem.Count))
                {
                    // and we somehow failed to re-add it to the source container...
                    // throw to the ground.
                    IThing thing = movingItem;
                    requestor.Tile.AddThing(ref thing, movingItem.Count);

                    // notify all spectator players of that tile.
                    Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, requestor.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, requestor.Location)), requestor.Location);

                    // call any collision events again.
                    if (requestor.Tile.HandlesCollision)
                    {
                        foreach (var itemWithCollision in requestor.Tile.ItemsWithCollision)
                        {
                            var collisionEvents = Game.Instance.EventsCatalog[EventType.Collision].Cast<CollisionEvent>();

                            var candidate =
                                collisionEvents.FirstOrDefault(
                                    e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId &&
                                         e.Setup(itemWithCollision, thing) && e.CanBeExecuted);

                            // Execute all actions.
                            candidate?.Execute();
                        }
                    }
                }
            }
            else
            {
                if (addedItem == null)
                {
                    return;
                }

                // added the new item to the slot
                IItem extraAddedItem;
                if (!requestor.Inventory.Add(addedItem, out extraAddedItem, FromSlot, movingItem.Count))
                {
                    // we exchanged or got some leftover item, place back in the source container at any index.
                    IThing remainderThing = extraAddedItem;

                    requestor.Tile.AddThing(ref remainderThing, remainderThing.Count);

                    // notify all spectator players of that tile.
                    Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, requestor.Tile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, requestor.Location)), requestor.Location);

                    // call any collision events again.
                    if (!requestor.Tile.HandlesCollision)
                    {
                        return;
                    }

                    foreach (var itemWithCollision in requestor.Tile.ItemsWithCollision)
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
}