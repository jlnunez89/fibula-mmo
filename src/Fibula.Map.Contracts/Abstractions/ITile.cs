// -----------------------------------------------------------------
// <copyright file="ITile.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map.Contracts.Abstractions
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Map.Contracts.Constants;
    using Fibula.Map.Contracts.Enumerations;

    /// <summary>
    /// Interface for all tiles.
    /// </summary>
    public interface ITile : IThingContainer
    {
        /// <summary>
        /// Gets the tile's creatures.
        /// </summary>
        IEnumerable<ICreature> Creatures { get; }

        /// <summary>
        /// Gets the flags from this tile.
        /// </summary>
        byte Flags { get; }

        /// <summary>
        /// Gets the last date and time that this tile was modified.
        /// </summary>
        DateTimeOffset LastModified { get; }

        /// <summary>
        /// Gets the single ground item that a tile may have.
        /// </summary>
        IItem Ground { get; }

        /// <summary>
        /// Gets the single liquid pool item that a tile may have.
        /// </summary>
        IItem LiquidPool { get; }

        /// <summary>
        /// Gets a value indicating whether items in this tile block throwing.
        /// </summary>
        bool BlocksThrow { get; }

        /// <summary>
        /// Gets a value indicating whether items in this tile block passing.
        /// </summary>
        bool BlocksPass { get; }

        /// <summary>
        /// Gets a value indicating whether items in this tile block laying.
        /// </summary>
        bool BlocksLay { get; }

        /// <summary>
        /// Gets the thing that is on top based on the tile's stack order.
        /// </summary>
        IThing TopThing { get; }

        /// <summary>
        /// Gets the creature that is on top based on the tile's stack order.
        /// </summary>
        ICreature TopCreature { get; }

        /// <summary>
        /// Gets the item that is on top based on the tile's stack order.
        /// </summary>
        IItem TopItem { get; }

        /// <summary>
        /// Attempts to get the tile's items to describe prioritized and ordered by their stack order.
        /// </summary>
        /// <param name="maxItemsToGet">The maximum number of items to include in the result.</param>
        /// <returns>The items in the tile, split by those which are fixed and those considered normal.</returns>
        (IEnumerable<IItem> fixedItems, IEnumerable<IItem> normalItems) GetItemsToDescribeByPriority(int maxItemsToGet = MapConstants.MaximumNumberOfThingsToDescribePerTile);

        /// <summary>
        /// Attempts to find an item in the tile with the given type.
        /// </summary>
        /// <param name="typeId">The type to look for.</param>
        /// <returns>The item with such id, null otherwise.</returns>
        IItem FindItemWithTypeId(ushort typeId);

        /// <summary>
        /// Attempts to get the position in the stack for the given <see cref="IThing"/>.
        /// </summary>
        /// <param name="thing">The thing to find.</param>
        /// <returns>The position in the stack for the <see cref="IThing"/>, or <see cref="byte.MaxValue"/> if its not found.</returns>
        byte GetStackOrderOfThing(IThing thing);

        /// <summary>
        /// Sets a flag on this tile.
        /// </summary>
        /// <param name="flag">The flag to set.</param>
        void SetFlag(TileFlag flag);

        /// <summary>
        /// Determines if this tile is considered to be blocking the path.
        /// </summary>
        /// <param name="avoidTypes">The damage types to avoid when checking for path blocking. By default, all types are considered path blocking.</param>
        /// <returns>True if the tile is considered path blocking, false otherwise.</returns>
        bool IsPathBlocking(byte avoidTypes = (byte)AvoidDamageType.All);
    }
}
