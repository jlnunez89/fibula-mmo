using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Movement.Policies
{
    internal class LocationNotAviodPolicy : IMovementPolicy
    {
        public Location Location { get; }
        public IThing Thing { get; }
        public uint RequestorId { get; }

        public string ErrorMessage => "Sorry, not possible.";

        public LocationNotAviodPolicy(uint requestingCreatureId, IThing thing, Location location)
        {
            RequestorId = requestingCreatureId;
            Thing = thing;
            Location = location;
        }

        public bool Evaluate()
        {
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);
            var destTile = Game.Instance.GetTileAt(Location);

            if (requestor == null || Thing == null || destTile == null)
            {
                // requestor being null means this was probably called from a script.
                // Not this policy's job to restrict this.
                return true;
            }

            return !(Thing is ICreature) || requestor == Thing || destTile.CanBeWalked();
        }
    }
}