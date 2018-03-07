using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Movement.Policies
{
    internal class GrabberHasEnoughCarryStrengthPolicy : IMovementPolicy
    {
        public IThing ThingPicking { get; }
        public IThing ThingDropping { get; }
        public uint PickerId { get; }

        public GrabberHasEnoughCarryStrengthPolicy(uint pickerId, IThing thingPicking, IThing thingDropping = null)
        {
            PickerId = pickerId;
            ThingPicking = thingPicking;
            ThingDropping = thingDropping is IContainer ? null : thingDropping; // We're actually trying to put this item in, so no dropping is happening.
        }
        public string ErrorMessage => "The object is too heavy.";

        public bool Evaluate()
        {
            var itemBeingPicked = ThingPicking as IItem;
            var itemBeingDropped = ThingDropping as IItem;
            var picker = PickerId == 0 ? null : Game.Instance.GetCreatureWithId(PickerId);

            if (itemBeingPicked == null || picker == null)
            {
                return false;
            }

            // TODO: PlayerId special access check
            return //(this.PlayerId is IPlayer && (this.PlayerId as IPlayer).AccessLevel > 0) ||
                picker.CarryStrength - itemBeingPicked.Weight + (itemBeingDropped?.Weight ?? 0) >= 0;
        }
    }
}