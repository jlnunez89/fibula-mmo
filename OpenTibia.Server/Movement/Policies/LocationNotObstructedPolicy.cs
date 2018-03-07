using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Movement.Policies
{
    internal class LocationNotObstructedPolicy : IMovementPolicy
    {
        public Location Location { get; }
        public IThing Thing { get; }
        public uint RequestorId { get; }

        public string ErrorMessage => "There is not enough room.";

        public LocationNotObstructedPolicy(uint requestingCreatureId, IThing thing, Location location)
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

            // creature trying to land on a blocking item.
            if (destTile.BlocksPass && Thing is ICreature)
            {
                return false;
            }

            var thingAsItem = Thing as IItem;

            if (thingAsItem != null)
            {
                if (destTile.BlocksLay || thingAsItem.BlocksPass && destTile.BlocksPass)
                {
                    return false;
                }
            }

            return true;
        }
    }
}