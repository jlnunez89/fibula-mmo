// -----------------------------------------------------------------
// <copyright file="ITile.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
    using Fibula.Map.Contracts.Enumerations;

    /// <summary>
    /// Interface for all tiles.
    /// </summary>
    public interface ITile : IThingContainer
    {
        /// <summary>
        /// Gets the count of creatures in this tile.
        /// </summary>
        int CreatureCount { get; }

        /// <summary>
        /// Gets the tile's creature ids.
        /// </summary>
        IEnumerable<uint> CreatureIds { get; }

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
        /// Gets the tile's normal items.
        /// </summary>
        IEnumerable<IItem> Items { get; }

        /// <summary>
        /// Gets the tile's 'stay-on-top' items.
        /// </summary>
        IEnumerable<IItem> StayOnTopItems { get; }

        /// <summary>
        /// Gets the tile's 'stay-on-bottom' items.
        /// </summary>
        IEnumerable<IItem> StayOnBottomItems { get; }

        /// <summary>
        /// Gets a value indicating whether this tile has events that are triggered via separation events.
        /// </summary>
        bool HasSeparationEvents { get; }

        /// <summary>
        /// Gets any items in the tile that have a separation event flag.
        /// </summary>
        IEnumerable<IItem> ItemsWithSeparation { get; }

        /// <summary>
        /// Gets a value indicating whether this tile has events that are triggered via collision evaluation.
        /// </summary>
        bool HasCollisionEvents { get; }

        /// <summary>
        /// Gets any items in the tile that have a collision event flag.
        /// </summary>
        IEnumerable<IItem> ItemsWithCollision { get; }

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
        /// Checks if the tile has an item with the given type.
        /// </summary>
        /// <param name="typeId">The type to check for.</param>
        /// <returns>True if the tile contains at least one item with such id, false otherwise.</returns>
        bool HasItemWithId(ushort typeId);

        /// <summary>
        /// Attempts to find an item in the tile with the given type.
        /// </summary>
        /// <param name="typeId">The type to look for.</param>
        /// <returns>The item with such id, null otherwise.</returns>
        IItem FindItemWithId(ushort typeId);

        /// <summary>
        /// Attempts to get the position in the stack for the given type id.
        /// </summary>
        /// <param name="typeId">The type id of the item to find.</param>
        /// <returns>The position in the stack for the item found, or <see cref="byte.MaxValue"/> if it's not found.</returns>
        byte GetPositionOfItemWithId(ushort typeId);

        /// <summary>
        /// Attempts to get the position in the stack for the given <see cref="IThing"/>.
        /// </summary>
        /// <param name="thing">The thing to find.</param>
        /// <returns>The position in the stack for the <see cref="IThing"/>, or <see cref="byte.MaxValue"/> if its not found.</returns>
        byte GetStackPositionOfThing(IThing thing);

        /// <summary>
        /// Attempts to get the tile's top <see cref="IThing"/> depending on the given order.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="order">The order in the stack to return.</param>
        /// <returns>A reference to the <see cref="IThing"/>, or null if nothing corresponds to that position.</returns>
        IThing GetTopThingByOrder(ICreatureFinder creatureFinder, byte order);

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