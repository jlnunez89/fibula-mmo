using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Movement.Policies
{
    internal class ContainerHasEnoughCapacityPolicy : IMovementPolicy
    {
        public IContainer TargetContainer { get; }

        public string ErrorMessage => "There is not enough room.";

        public ContainerHasEnoughCapacityPolicy(IContainer destinationContainer)
        {
            TargetContainer = destinationContainer;
        }

        public bool Evaluate()
        {
            return TargetContainer != null && TargetContainer?.Volume - TargetContainer.Content.Count > 0;
        }
    }
}