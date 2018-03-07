using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Movement.Policies
{
    internal class LocationsAreDistantByPolicy : IMovementPolicy
    {
        public Location FirstLocation { get; }
        public Location SecondLocation { get; }
        public byte Distance { get; }
        public bool SameFloorOnly { get; }

        public string ErrorMessage => "The destination is too far away.";

        public LocationsAreDistantByPolicy(Location locationOne, Location locationTwo, byte distance = 1, bool sameFloorOnly = false)
        {
            FirstLocation = locationOne;
            SecondLocation = locationTwo;
            Distance = distance;
            SameFloorOnly = sameFloorOnly;
        }

        public bool Evaluate()
        {
            var locationDiff = (FirstLocation - SecondLocation).MaxValueIn2D;
            var sameFloor = FirstLocation.Z == SecondLocation.Z;
            
            if (locationDiff <= Distance && (!SameFloorOnly || sameFloor))
            {
                // The thing is no longer in this position.

                return true;
            }

            return false;
        }
    }
}