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
    public interface IContainerItem : IItem, ICylinder
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
        /// Marks this container as oppened by a creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature that is opening this container.</param>
        /// <param name="containerId">The id which the creature is proposing to label this container with.</param>
        /// <returns>The id of the container which this container is or will be known to this creature.</returns>
        /// <remarks>The id returned may not match the one supplied if the container was already opened by this creature before.</remarks>
        byte BeginTracking(uint creatureId, byte containerId);

        /// <summary>
        /// Marks this container as closed by a creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature that is closing this container.</param>
        void EndTracking(uint creatureId);

        /// <summary>
        /// Checks if this container is being tracked as opened a creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature.</param>
        /// <param name="containerId">The id which the creature is tracking this container with.</param>
        /// <returns>True if this container is being tracked by the creature, false otherwise.</returns>
        bool IsTracking(uint creatureId, out byte containerId);

        /// <summary>
        /// Counts the amount of the specified content item at a given index within this container.
        /// </summary>
        /// <param name="index">The index at which to count.</param>
        /// <param name="typeIdExpected">Optional. The type id of the content item expected to be found.</param>
        /// <returns>The count of the item at the index. If <paramref name="typeIdExpected"/> is specified, the value returned will only count if the type matches, otherwise -1 will be returned.</returns>
        sbyte CountAmountAt(byte index, ushort typeIdExpected = 0);
    }
}
