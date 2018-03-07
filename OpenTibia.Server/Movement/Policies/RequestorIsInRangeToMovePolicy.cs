using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Movement.Policies
{
    internal class RequestorIsInRangeToMovePolicy : IMovementPolicy
    {
        public uint RequestorId { get; }
        public Location FromLocation { get; }

        public string ErrorMessage => "You are too far away.";

        public RequestorIsInRangeToMovePolicy(uint requestorId, Location movingFrom)
        {
            RequestorId = requestorId;
            FromLocation = movingFrom;
        }

        public bool Evaluate()
        {
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (requestor == null)
            {
                // script called, probably
                return true;
            }
            
            return (requestor.Location - FromLocation).MaxValueIn2D <= 1;
        }
    }
}