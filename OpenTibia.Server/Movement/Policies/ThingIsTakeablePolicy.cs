using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Movement.Policies
{
    internal class ThingIsTakeablePolicy : IMovementPolicy
    {
        public IThing Thing { get; }
        public uint GrabberId { get; }

        public string ErrorMessage => "You may not move this object.";

        public ThingIsTakeablePolicy(uint grabberId, IThing thingMoving)
        {
            GrabberId = grabberId;
            Thing = thingMoving;
        }

        public bool Evaluate()
        {
            var grabber = GrabberId == 0 ? null : Game.Instance.GetCreatureWithId(GrabberId);

            if (grabber == null || Thing == null)
            {
                return false;
            }

            var item = Thing as IItem;

            // TODO: GrabberId access level?

            return item != null && item.Type.Flags.Contains(ItemFlag.Take);
        }
    }
}