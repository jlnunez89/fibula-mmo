using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Movement.Policies
{
    internal class GrabberHasContainerOpenPolicy : IMovementPolicy
    {
        public IContainer TargetContainer { get; }
        public uint GrabberId { get; }

        public GrabberHasContainerOpenPolicy(uint grabberId, IContainer destinationContainer)
        {
            GrabberId = grabberId;
            TargetContainer = destinationContainer;
        }

        public string ErrorMessage => "Sorry, not possible.";

        public bool Evaluate()
        {
            var grabber = GrabberId == 0 ? null : Game.Instance.GetCreatureWithId(GrabberId);

            if (grabber == null || TargetContainer == null)
            {
                return false;
            }
            
            return !(grabber is IPlayer) || (grabber as IPlayer)?.GetContainerId(TargetContainer) >= 0;
        }
    }
}