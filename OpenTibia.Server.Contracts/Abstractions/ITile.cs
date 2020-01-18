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

    public interface ITile : ICylinder
    {
        int CreatureCount { get; }

        IEnumerable<uint> CreatureIds { get; }

        byte Flags { get; }

        DateTimeOffset LastModified { get; }

        IItem Ground { get; }

        IEnumerable<IItem> Items { get; }

        IEnumerable<IItem> StayOnTopItems { get; }

        IEnumerable<IItem> StayOnBottomItems { get; }

        bool HasSeparationEvents { get; }

        IEnumerable<IItem> ItemsWithSeparation { get; }

        bool HasCollisionEvents { get; }

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

        bool HasItemWithId(ushort typeId);

        IItem FindItemWithId(ushort typeId);

        byte GetStackPositionOfThing(IThing thing);

        /// <summary>
        /// Attempts to get the tile's top <see cref="IThing"/> depending on the given order.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="order">The order in the stack to return.</param>
        /// <returns>A reference to the <see cref="IThing"/>, or null if nothing corresponds to that position.</returns>
        IThing GetTopThingByOrder(ICreatureFinder creatureFinder, byte order);

        void SetFlag(TileFlag flag);

        bool IsPathBlocking(byte avoidTypes = (byte)AvoidDamageType.All);
    }
}