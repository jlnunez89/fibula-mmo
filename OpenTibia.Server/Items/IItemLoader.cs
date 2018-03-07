using System.Collections.Generic;

namespace OpenTibia.Server.Items
{
    public interface IItemLoader
    {
        Dictionary<ushort, ItemType> Load(string objectsFileName);
    }
}
