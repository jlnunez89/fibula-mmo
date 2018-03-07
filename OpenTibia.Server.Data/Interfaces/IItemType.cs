namespace OpenTibia.Server.Data.Interfaces
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Data.Contracts;

    public interface IItemType
    {
        ushort TypeId { get; }

        string Name { get; }

        string Description { get; }

        ISet<ItemFlag> Flags { get; }

        IDictionary<ItemAttribute, IConvertible> DefaultAttributes { get; } // TODO: get rid of this and add all attributes as properties.

        ushort ClientId { get; }
    }
}
