using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Movement.Policies
{
    internal class SlotHasContainerAndContainerHasEnoughCapacityPolicy : IMovementPolicy
    {
        public IItem ItemInSlot { get; }
        public uint PlayerId { get; }

        public SlotHasContainerAndContainerHasEnoughCapacityPolicy(uint playerId, IItem itemInSlot)
        {
            PlayerId = playerId;
            ItemInSlot = itemInSlot;
        }

        public string ErrorMessage => "The container is full.";

        public bool Evaluate()
        {
            var player = PlayerId == 0 ? null : Game.Instance.GetCreatureWithId(PlayerId);

            if (player == null)
            {
                return false;
            }

            if (ItemInSlot == null)
            {
                return true;
            }

            var itemAsContainer = ItemInSlot as IContainer;
            return !ItemInSlot.IsContainer || itemAsContainer != null && itemAsContainer.Content.Count < itemAsContainer.Volume;
        }
    }
}