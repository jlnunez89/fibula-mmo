using System.Collections.Generic;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Monsters
{
    public interface IMonsterLoader
    {
        Dictionary<ushort, MonsterType> LoadMonsters(string loadFromPath);

        IEnumerable<Spawn> LoadSpawns(string spawnsFileName);
    }
}
