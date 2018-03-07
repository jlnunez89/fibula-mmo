using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Movement.Policies
{
    internal class UnpassItemsInRangePolicy : IMovementPolicy
    {
        public Location ToLocation { get; }
        public IThing Thing { get; }
        public uint MoverId { get; }

        public string ErrorMessage => "Sorry, not possible.";

        public UnpassItemsInRangePolicy(uint moverId, IThing thingMoving, Location targetLoc)
        {
            MoverId = moverId;
            Thing = thingMoving;
            ToLocation = targetLoc;
        }

        public bool Evaluate()
        {
            var mover = MoverId == 0 ? null : Game.Instance.GetCreatureWithId(MoverId);
            var item = Thing as IItem;

            if (item == null || mover == null || !item.Type.Flags.Contains(ItemFlag.Unpass))
            {
                // MoverId being null means this is probably a script's action.
                // Policy does not apply to this thing.
                return true;
            }

            var locDiff = mover.Location - ToLocation;
            
            return locDiff.Z == 0 && locDiff.MaxValueIn2D <= 2;
        }
    }
}