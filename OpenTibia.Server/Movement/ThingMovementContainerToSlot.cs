using System;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;
using OpenTibia.Server.Movement.Policies;
using OpenTibia.Server.Notifications;

namespace OpenTibia.Server.Movement
{
    internal class ThingMovementContainerToSlot : MovementBase
    {
        public Location FromLocation { get; }
        public IContainer FromContainer { get; }
        public byte FromIndex { get; }

        public Location ToLocation { get; }
        public byte ToSlot { get; }

        public IThing Thing { get; }
        public byte Count { get; }

        public ThingMovementContainerToSlot(uint creatureRequestingId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
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
            ToSlot = (byte)ToLocation.Slot;

            var droppingItem = requestor.Inventory?[ToSlot];
            
            if (FromContainer != null && FromContainer.HolderId != requestor.CreatureId)
            {
                Policies.Add(new GrabberHasEnoughCarryStrengthPolicy(RequestorId, Thing, droppingItem));
            }

            Policies.Add(new SlotHasContainerAndContainerHasEnoughCapacityPolicy(RequestorId, droppingItem));
            Policies.Add(new GrabberHasContainerOpenPolicy(RequestorId, FromContainer));
            Policies.Add(new ContainerHasItemAndEnoughAmountPolicy(Thing as IItem, FromContainer, FromIndex, Count));
        }

        public override void Perform()
        {
            IItem addedItem;
            var updateItem = Thing as IItem;
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (FromContainer == null || updateItem == null || requestor == null)
            {
                return;
            }
            
            // attempt to remove from the source container
            if (!FromContainer.RemoveContent(updateItem.Type.TypeId, FromIndex, Count, out addedItem))
            {
                return;
            }

            if (addedItem != null)
            {
                updateItem = addedItem;
            }

            IThing currentThing = null;
            // attempt to place the intended item at the slot.
            if (!requestor.Inventory.Add(updateItem, out addedItem, ToSlot, Count))
            {
                // Something went wrong, add back to the source container...
                if (FromContainer.AddContent(updateItem, 0xFF))
                {
                    return;
                }

                // and we somehow failed to re-add it to the source container...
                // throw to the ground.
                currentThing = updateItem;
            }
            else
            {
                // added the new item to the slot
                if (addedItem == null)
                {
                    return;
                }

                // we exchanged or got some leftover item, place back in the source container at any index.
                if (FromContainer.AddContent(addedItem, 0xFF))
                {
                    return;
                }

                // and we somehow failed to re-add it to the source container...
                // throw to the ground.
                currentThing = addedItem;
            }

            requestor.Tile.AddThing(ref currentThing, currentThing.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, requestor.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, requestor.Location)), requestor.Location);
        }
    }
}