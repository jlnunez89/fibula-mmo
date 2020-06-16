// -----------------------------------------------------------------
// <copyright file="IMap.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Map.Contracts.Abstractions
{
    using System.Buffers;
    using System.Collections.Generic;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Server.Contracts.Abstractions;
    using Fibula.Server.Contracts.Structs;
    using Fibula.Server.Map.Contracts;

    /// <summary>
    /// Interface for a map.
    /// </summary>
    public interface IMap : ITileAccessor
    {
        /// <summary>
        /// The maximum number of <see cref="IThing"/>s to describe per tile.
        /// </summary>
        public const int MaximumNumberOfThingsToDescribePerTile = 9;

        /// <summary>
        /// The default window size in the X coordinate.
        /// </summary>
        public const byte DefaultWindowSizeX = 18;

        /// <summary>
        /// The default window size in the Y coordinate.
        /// </summary>
        public const byte DefaultWindowSizeY = 14;

        /// <summary>
        /// Gets the description of the map as seen by the given <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player for which the map is being described.</param>
        /// <param name="centerLocation">The center location from which to get the description.</param>
        /// <returns>The description bytes.</returns>
        ReadOnlySequence<byte> DescribeForPlayer(IPlayer player, Location centerLocation);

        /// <summary>
        /// Gets the specified window's description of the map as seen by the given <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player for which the map is being described.</param>
        /// <param name="fromX">The coordinate X value at which the window of description begins.</param>
        /// <param name="fromY">The coordinate Y value at which the window of description begins.</param>
        /// <param name="currentZ">The coordinate Z value at which the window of description begins.</param>
        /// <param name="windowSizeX">The size of the window on the X axis.</param>
        /// <param name="windowSizeY">The size of the window on the Y axis.</param>
        /// <returns>The description bytes.</returns>
        ReadOnlySequence<byte> DescribeForPlayer(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, byte windowSizeX = DefaultWindowSizeX, byte windowSizeY = DefaultWindowSizeY);

        /// <summary>
        /// Gets the specified window's description of the map as seen by the given <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player for which the map is being described.</param>
        /// <param name="fromX">The coordinate X value at which the window of description begins.</param>
        /// <param name="toX">The coordinate X value at which the window of description ends.</param>
        /// <param name="fromY">The coordinate Y value at which the window of description begins.</param>
        /// <param name="toY">The coordinate Y value at which the window of description ends.</param>
        /// <param name="fromZ">The coordinate Z value at which the window of description begins.</param>
        /// <param name="toZ">The coordinate Z value at which the window of description ends.</param>
        /// <param name="additionalOffsetZ">Optional. An additional Z offset used mainly for partial floor changing windows. Defaults to 0.</param>
        /// <returns>The description bytes.</returns>
        ReadOnlySequence<byte> DescribeForPlayer(IPlayer player, ushort fromX, ushort toX, ushort fromY, ushort toY, sbyte fromZ, sbyte toZ, sbyte additionalOffsetZ = 0);

        /// <summary>
        /// Gets the description segments of a tile as seen by the given <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player for which the tile is being described.</param>
        /// <param name="location">The location of the tile being described.</param>
        /// <returns>A collection of description segments from the tile.</returns>
        IEnumerable<MapDescriptionSegment> DescribeTileForPlayer(IPlayer player, Location location);
    }
}