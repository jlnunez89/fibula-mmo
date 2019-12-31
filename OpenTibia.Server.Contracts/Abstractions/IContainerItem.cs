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
    using System.Collections.Generic;

    /// <summary>
    /// Interface for items that are containers for other items.
    /// </summary>
    public interface IContainerItem : IItem
    {
        /// <summary>
        /// The default container capacity value.
        /// </summary>
        const int DefaultContainerCapacity = 8;

        /// <summary>
        /// A delegate to invoke when new content is added to this container.
        /// </summary>
        event OnContentAdded OnContentAdded;

        /// <summary>
        /// A delegate to invoke when content is updated in this container.
        /// </summary>
        event OnContentUpdated OnContentUpdated;

        /// <summary>
        /// A delegate to invoke when content is removed from this container.
        /// </summary>
        event OnContentRemoved OnContentRemoved;

        /// <summary>
        /// Gets the mapping of player ids to container ids for which this container is known to be opened.
        /// </summary>
        IDictionary<uint, byte> OpenedBy { get; }

        /// <summary>
        /// Gets the collection of items contained in this container.
        /// </summary>
        IList<IItem> Content { get; }

        /// <summary>
        /// Attempts to retrieve an item from the contents of this container based on a given index.
        /// </summary>
        /// <param name="index">The index to retrieve.</param>
        /// <returns>The item retrieved, if any, or null.</returns>
        IItem this[int index] { get; }

        /// <summary>
        /// Gets the capacity of this container.
        /// </summary>
        byte Capacity { get; }

        /// <summary>
        /// Attempts to add an item to this container.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="originalItem">The item to add.</param>
        /// <param name="index">Optional. The zero-based index at which to add the item. Defaults to 0xFF, which won't match any item.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the item may be returned.</returns>
        (bool result, IItem remainderItem) AddContent(IItemFactory itemFactory, IItem originalItem, byte index = 0xFF);

        /// <summary>
        /// Attempts to remove an item from this container.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="itemId">The id of the item to look for to remove.</param>
        /// <param name="index">The index at which to look for the item to remove.</param>
        /// <param name="amount">The amount of the item to remove.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the item may be returned.</returns>
        (bool result, IItem remainderItem) RemoveContent(IItemFactory itemFactory, ushort itemId, byte index, byte amount);

        /// <summary>
        /// Marks this container as oppened by a creature.
        /// </summary>
        /// <param name="creatureOpeningId">The id of the creature that is opening this container.</param>
        /// <param name="containerId">The id which the creature is proposing to label this container with.</param>
        /// <returns>The id of the container which this container is or will be known to this creature.</returns>
        /// <remarks>The id returned may not match the one supplied if the container was already opened by this creature before.</remarks>
        byte Open(uint creatureOpeningId, byte containerId);

        /// <summary>
        /// Marks this container as closed by a creature.
        /// </summary>
        /// <param name="creatureClosingId">The id of the creature that is closing this container.</param>
        void Close(uint creatureClosingId);

        /// <summary>
        /// Counts the amount of the specified content item at a given index within this container.
        /// </summary>
        /// <param name="index">The index at which to count.</param>
        /// <param name="typeIdExpected">Optional. The type id of the content item expected to be found.</param>
        /// <returns>The count of the item at the index. If <paramref name="typeIdExpected"/> is specified, the value returned will only count if the type matches, otherwise -1 will be returned.</returns>
        sbyte CountAmountAt(byte index, ushort typeIdExpected = 0);

        /// <summary>
        /// Retrieves the id of this container as known to a creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature.</param>
        /// <returns>The id of this container if it was found for the creature, -1 otherwise.</returns>
        sbyte AsKnownTo(uint creatureId);
    }
}
