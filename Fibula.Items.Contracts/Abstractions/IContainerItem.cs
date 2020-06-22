// -----------------------------------------------------------------
// <copyright file="IContainerItem.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Items.Contracts.Delegates;

    /// <summary>
    /// Interface for <see cref="IItem"/>s that are containers for other <see cref="IItem"/>s.
    /// </summary>
    public interface IContainerItem : IItem, IThingContainer
    {
        /// <summary>
        /// The default container capacity value.
        /// </summary>
        const int DefaultContainerCapacity = 8;

        /// <summary>
        /// A delegate to invoke when new content is added to this container.
        /// </summary>
        event OnContentAdded ContentAdded;

        /// <summary>
        /// A delegate to invoke when content is updated in this container.
        /// </summary>
        event OnContentUpdated ContentUpdated;

        /// <summary>
        /// A delegate to invoke when content is removed from this container.
        /// </summary>
        event OnContentRemoved ContentRemoved;

        /// <summary>
        /// Gets the collection of items contained in this container.
        /// </summary>
        IList<IItem> Content { get; }

        /// <summary>
        /// Gets the capacity of this container.
        /// </summary>
        byte Capacity { get; }

        /// <summary>
        /// Attempts to retrieve an item from the contents of this container based on a given index.
        /// </summary>
        /// <param name="index">The index to retrieve.</param>
        /// <returns>The item retrieved, if any, or null.</returns>
        IItem this[int index] { get; }

        /// <summary>
        /// Counts the amount of the specified content item at a given index within this container.
        /// </summary>
        /// <param name="index">The index at which to count.</param>
        /// <param name="typeIdExpected">Optional. The type id of the content item expected to be found.</param>
        /// <returns>The count of the item at the index. If <paramref name="typeIdExpected"/> is specified, the value returned will only count if the type matches, otherwise -1 will be returned.</returns>
        sbyte CountAmountAt(byte index, ushort typeIdExpected = 0);

        /// <summary>
        /// Checks that this item's parents are not this same item.
        /// </summary>
        /// <param name="item">The parent item to check.</param>
        /// <returns>True if this item is child of any item in the parent hierarchy, false otherwise.</returns>
        bool IsChildOf(IItem item);
    }
}
