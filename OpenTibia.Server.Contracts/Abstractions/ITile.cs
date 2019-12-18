// -----------------------------------------------------------------
// <copyright file="ITile.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Enumerations;

    public interface ITile
    {
        int CreatureCount { get; }

        IEnumerable<uint> CreatureIds { get; }

        IEnumerable<IItem> Items { get; }

        byte Flags { get; }

        IItem Ground { get; }

        DateTimeOffset LastModified { get; }

        IEnumerable<IItem> StayOnTopItems { get; }

        IEnumerable<IItem> StayOnBottomItems { get; }

        bool HasSeparationEvents { get; }

        IEnumerable<IItem> ItemsWithSeparation { get; }

        bool HasCollisionEvents { get; }

        IEnumerable<IItem> ItemsWithCollision { get; }

        void AddThing(IItemFactory itemFactory, ref IThing thing, byte count = 1);

        bool RemoveThing(IItemFactory itemFactory, ref IThing thing, byte count = 1);

        bool HasItemWithId(ushort itemId);

        byte GetStackPositionOfThing(IThing thing);

        /// <summary>
        /// Attempts to get the tile's top <see cref="IThing"/> depending on the given order.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="order">The order in the stack to return.</param>
        /// <returns>A reference to the <see cref="IThing"/>, or null if nothing corresponds to that position.</returns>
        IThing GetTopThingByOrder(ICreatureFinder creatureFinder, byte order);

        void SetFlag(TileFlag flag);
    }
}