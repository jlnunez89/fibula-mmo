using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Movement.Policies
{
    internal class LocationsMatchPolicy : LocationsAreDistantByPolicy
    {
        public LocationsMatchPolicy(Location locationOne, Location locationTwo, byte distance = 0, bool sameFloorOnly = true) 
            : base(locationOne, locationTwo, distance, sameFloorOnly)
        {
            // Nothing else...
        }
    }
}