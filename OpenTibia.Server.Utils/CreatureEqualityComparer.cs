using System.Collections.Generic;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Utilities
{
    public class CreatureEqualityComparer : IEqualityComparer<ICreature>
    {
        public bool Equals(ICreature x, ICreature y)
        {
            return x.CreatureId == y.CreatureId;
        }

        public int GetHashCode(ICreature obj)
        {
            return (int)obj.CreatureId;
        }
    }
}
