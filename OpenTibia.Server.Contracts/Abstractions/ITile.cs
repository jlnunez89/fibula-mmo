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

        IEnumerable<IItem> DownItems { get; }

        byte Flags { get; }

        IItem Ground { get; }

        DateTimeOffset LastModified { get; }

        IEnumerable<IItem> TopItems1 { get; }

        IEnumerable<IItem> TopItems2 { get; }

        void AddThing(IItemFactory itemFactory, ref IThing thing, byte count = 1);

        byte GetStackPositionOfThing(IThing thing);

        IThing GetThingAtStackPosition(ICreatureFinder creatureFinder, byte stackPosition);

        bool RemoveThing(IItemFactory itemFactory, ref IThing thing, byte count = 1);

        void SetFlag(TileFlag flag);
    }
}