// -----------------------------------------------------------------
// <copyright file="IInventory.cs" company="2Dudes">
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
    /// <summary>
    /// Interface for a creature that keeps an inventory of <see cref="IItem"/>s in itself, and the properties it imbues the owner <see cref="ICreature"/> with.
    /// </summary>
    public interface IInventory
    {
        /// <summary>
        /// Gets a reference to the owner of this inventory.
        /// </summary>
        ICreature Owner { get; }

        /// <summary>
        /// Gets the <see cref="IItem"/> at a given position of this inventory.
        /// </summary>
        /// <param name="idx">The index or position where to get the item from.</param>
        /// <returns>The <see cref="IItem"/>, if any was found.</returns>
        IItem this[byte idx] { get; }

        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="extraItem">A remainder or swapped item that resulted from adding the first.</param>
        /// <param name="positionByte">Optional. The position or index at which to add the item.</param>
        /// <param name="count">Optional. The count of the item to add.</param>
        /// <param name="lossProbability">Optional. A probability [1, 10000] for item loss from this inventory, upon that event happening.</param>
        /// <returns>True if the item was successfully added to the inventory, false otherwise.</returns>
        bool Add(IItem item, out IItem extraItem, byte positionByte = 0xFF, byte count = 1, ushort lossProbability = 300);

        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        /// <param name="positionByte">The position at which to look for and remove the item.</param>
        /// <param name="count">The count of this item to remove.</param>
        /// <param name="wasPartial">A value indicating wether the removal meant a partial removal only.</param>
        /// <returns>The item removed.</returns>
        IItem Remove(byte positionByte, byte count, out bool wasPartial);

        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        /// <param name="itemId">The id of the item to look for and remove.</param>
        /// <param name="count">The count of this item to remove.</param>
        /// <param name="wasPartial">A value indicating wether the removal meant a partial removal only.</param>
        /// <returns>The item removed.</returns>
        IItem Remove(ushort itemId, byte count, out bool wasPartial);

        // TODO: add special properties given by items here.
    }
}
