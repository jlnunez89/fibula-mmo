// -----------------------------------------------------------------
// <copyright file="IInventory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Abstractions
{
    using Fibula.Items.Contracts.Abstractions;

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
        /// <param name="position">The position where to get the item from.</param>
        /// <returns>The <see cref="IItem"/>, if any was found.</returns>
        IItem this[byte position] { get; }

        // TODO: add special properties given by items here.
    }
}
