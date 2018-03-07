using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Movement.Policies
{
    internal class LocationHasTileWithGroundPolicy : IMovementPolicy
    {
        public Location Location { get; }

        public string ErrorMessage => "There is not enough room.";

        public LocationHasTileWithGroundPolicy(Location location)
        {
            Location = location;
        }

        public bool Evaluate()
        {
            return Game.Instance.GetTileAt(Location)?.Ground != null;
        }
    }
}