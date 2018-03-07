using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Movement.Policies
{
    internal class ContainerHasItemAndEnoughAmountPolicy : IMovementPolicy
    {
        public IItem ItemToCheck { get; }
        public IContainer FromContainer { get; }
        public byte FromIndex { get; }
        public byte Count { get; }

        public string ErrorMessage => "There is not enough quantity.";

        public ContainerHasItemAndEnoughAmountPolicy(IItem itemToCheck, IContainer fromContainer, byte indexToCheck, byte countToCheck)
        {
            ItemToCheck = itemToCheck;
            FromContainer = fromContainer;
            FromIndex = indexToCheck;
            Count = countToCheck;
        }

        public bool Evaluate()
        {
            if (ItemToCheck == null || FromContainer == null)
            {
                return false;
            }

            return FromContainer.CountContentAmountAt(FromIndex, ItemToCheck.Type.TypeId) >= Count;
        }
    }
}