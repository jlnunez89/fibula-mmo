using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Movement.Policies
{
    internal class SlotContainsItemAndCountPolicy : IMovementPolicy
    {
        public uint RequestorId { get; }
        public IItem ItemMoving { get; }
        public byte Slot { get; }
        public byte Count { get; }

        public string ErrorMessage => "You are too far away.";

        public SlotContainsItemAndCountPolicy(uint requestorId, IItem movingItem, byte slot, byte count = 1)
        {
            RequestorId = requestorId;
            ItemMoving = movingItem;
            Slot = slot;
            Count = count;
        }

        public bool Evaluate()
        {
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (requestor == null || ItemMoving == null)
            {
                return false;
            }

            var itemAtSlot = requestor.Inventory?[Slot];

            return itemAtSlot != null && ItemMoving.Type.TypeId == itemAtSlot.Type.TypeId && itemAtSlot.Count >= Count;
        }
    }
}