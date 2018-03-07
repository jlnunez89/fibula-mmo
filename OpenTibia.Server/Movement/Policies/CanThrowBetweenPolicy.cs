using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Movement.Policies
{
    internal class CanThrowBetweenPolicy : IMovementPolicy
    {
        public Location FromLocation { get; }
        public Location ToLocation { get; }
        public bool CheckSight { get; }
        public uint RequestorId { get; }

        public string ErrorMessage => "You may not throw there.";

        public CanThrowBetweenPolicy(uint requestorId, Location fromLocation, Location toLocation, bool checkLineOfSight = true)
        {
            RequestorId = requestorId;
            FromLocation = fromLocation;
            ToLocation = toLocation;
            CheckSight = checkLineOfSight;
        }

        public bool Evaluate()
        {
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (requestor == null)
            {
                // Empty requestorId means not a creature generated event...
                return true;
            }

            return Game.Instance.CanThrowBetween(FromLocation, ToLocation, CheckSight);
        }
    }
}