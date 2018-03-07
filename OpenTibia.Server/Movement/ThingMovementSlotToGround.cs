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
    internal class ThingMovementSlotToGround : MovementBase
    {
        public Location FromLocation { get; }
        public byte FromSlot { get; }

        public Location ToLocation { get; }
        public ITile ToTile { get; }

        public IItem Item { get; }
        public byte Count { get; }

        public ThingMovementSlotToGround(uint creatureRequestingId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base (creatureRequestingId)
        {
            // intentionally left thing null check out. Handled by Perform().
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }
            
            FromLocation = fromLocation;
            FromSlot = (byte)fromLocation.Slot;

            ToLocation = toLocation;
            ToTile = Game.Instance.GetTileAt(ToLocation);

            Item = thingMoving as IItem;
            Count = count;
            
            if (requestor != null)
            {
                Policies.Add(new CanThrowBetweenPolicy(RequestorId, requestor.Location, ToLocation));
            }

            Policies.Add(new SlotContainsItemAndCountPolicy(creatureRequestingId, Item, FromSlot, Count));
            Policies.Add(new LocationNotObstructedPolicy(RequestorId, Item, ToLocation));
            Policies.Add(new LocationHasTileWithGroundPolicy(ToLocation));
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

            // add the remaining item to the destination tile.
            IThing thing = movingItem;
            ToTile.AddThing(ref thing, movingItem.Count);

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