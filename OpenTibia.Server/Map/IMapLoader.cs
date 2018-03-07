using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Map
{
    public interface IMapLoader
    {
        byte PercentageComplete { get; }

        //ITile[,,] LoadFullMap();

        ITile[,,] Load(int fromSectorX, int toSectorX, int fromSectorY, int toSectorY, byte fromSectorZ, byte toSectorZ);

        bool HasLoaded(int x, int y, byte z);
    }
}