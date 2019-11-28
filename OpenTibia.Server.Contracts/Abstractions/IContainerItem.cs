// -----------------------------------------------------------------
// <copyright file="IContainerItem.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for items that are containers for other items.
    /// </summary>
    public interface IContainerItem : IItem
    {
        event OnContentAdded OnContentAdded;

        event OnContentUpdated OnContentUpdated;

        event OnContentRemoved OnContentRemoved;

        IDictionary<Guid, byte> OpenedBy { get; }

        IList<IItem> Content { get; }

        byte Volume { get; }

        bool AddContent(IItem item, byte index);

        bool RemoveContent(ushort itemId, byte index, byte count, out IItem splitItem);

        sbyte CountContentAmountAt(byte fromIndex, ushort itemIdExpected = 0);

        byte Open(uint creatureOpeningId, byte containerId);

        void Close(uint creatureClosingId);

        sbyte GetIdFor(uint creatureId);
    }
}
