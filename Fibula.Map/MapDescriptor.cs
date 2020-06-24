// -----------------------------------------------------------------
// <copyright file="MapDescriptor.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map
{
    using System;
    using System.Buffers;
    using System.Linq;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Constants;

    /// <summary>
    /// Class that implements a standard map descriptor.
    /// </summary>
    public class MapDescriptor : IMapDescriptor
    {
        /// <summary>
        /// A reference to the map.
        /// </summary>
        private readonly IMap map;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapDescriptor"/> class.
        /// </summary>
        /// <param name="map">The reference to the map in use.</param>
        public MapDescriptor(IMap map)
        {
            map.ThrowIfNull(nameof(map));

            this.map = map;
        }

        /// <summary>
        /// Gets the description bytes of the map in behalf of a given player at a given location.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="location">The center location from which the description is being retrieved.</param>
        /// <returns>A sequence of bytes representing the description.</returns>
        public ReadOnlySequence<byte> DescribeAt(IPlayer player, Location location)
        {
            player.ThrowIfNull(nameof(player));

            return this.map.DescribeForPlayer(player, location);
        }

        /// <summary>
        /// Gets the description bytes of the map in behalf of a given player for the specified window.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="startX">The starting X coordinate of the window.</param>
        /// <param name="startY">The starting Y coordinate of the window.</param>
        /// <param name="startZ">The starting Z coordinate of the window.</param>
        /// <param name="endZ">The ending Z coordinate of the window.</param>
        /// <param name="windowSizeX">The size of the window in X.</param>
        /// <param name="windowSizeY">The size of the window in Y.</param>
        /// <param name="startingZOffset">Optional. A starting offset for Z.</param>
        /// <returns>A sequence of bytes representing the description.</returns>
        public ReadOnlySequence<byte> DescribeWindow(IPlayer player, ushort startX, ushort startY, sbyte startZ, sbyte endZ, byte windowSizeX = MapConstants.DefaultWindowSizeX, byte windowSizeY = MapConstants.DefaultWindowSizeY, sbyte startingZOffset = 0)
        {
            player.ThrowIfNull(nameof(player));

            return this.map.DescribeForPlayer(player, startX, (ushort)(startX + windowSizeX), startY, (ushort)(startY + windowSizeY), startZ, endZ, startingZOffset);
        }

        /// <summary>
        /// Gets the description bytes of a single tile of the map in behalf of a given player at a given location.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="location">The location from which the description of the tile is being retrieved.</param>
        /// <returns>A sequence of bytes representing the tile's description.</returns>
        public ReadOnlySequence<byte> DescribeTile(IPlayer player, Location location)
        {
            player.ThrowIfNull(nameof(player));

            if (location.Type != LocationType.Map)
            {
                throw new ArgumentException($"Invalid location {location}.", nameof(location));
            }

            var firstSegment = new MapDescriptionSegment(ReadOnlyMemory<byte>.Empty);

            MapDescriptionSegment lastSegment = firstSegment;

            var segmentsFromTile = this.map.DescribeTileForPlayer(player, location);

            // See if we actually have segments to append.
            if (segmentsFromTile != null && segmentsFromTile.Any())
            {
                foreach (var segment in segmentsFromTile)
                {
                    lastSegment.Append(segment);
                    lastSegment = segment;
                }
            }

            // HACK: add a last segment to seal the deal.
            lastSegment = lastSegment.Append(ReadOnlyMemory<byte>.Empty);

            return new ReadOnlySequence<byte>(firstSegment, 0, lastSegment, 0);
        }
    }
}
