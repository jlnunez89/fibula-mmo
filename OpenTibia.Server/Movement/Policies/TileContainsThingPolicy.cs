using System;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Movement.Policies
{
    internal class TileContainsThingPolicy : IMovementPolicy
    {
        public Location Location { get; }
        public IThing Thing { get; }
        public byte Count { get; }

        public TileContainsThingPolicy(IThing thing, Location location, byte count = 1)
        {
            if (count == 0 || count > 100)
            {
                throw new ArgumentException($"Invalid count {count}.", nameof(count));
            }

            Thing = thing;
            Count = count;
            Location = location;
        }

        public string ErrorMessage => "Sorry, not possible.";

        public bool Evaluate()
        {
            if (Thing == null)
            {
                return false;
            }

            var sourceTile = Game.Instance.GetTileAt(Location);

            if (sourceTile == null || !sourceTile.HasThing(Thing))
            {
                // This tile no longer has the thing, or it's obstructed (i.e. someone placed something on top of it).
                return false;
            }

            return true;
        }
    }
}