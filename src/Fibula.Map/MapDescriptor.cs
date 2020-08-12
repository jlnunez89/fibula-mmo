// -----------------------------------------------------------------
// <copyright file="MapDescriptor.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Constants;
    using Serilog;

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
        /// A reference to the tile descriptor in use.
        /// </summary>
        private readonly IProtocolTileDescriptor tileDescriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapDescriptor"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger to use.</param>
        /// <param name="map">The reference to the map in use.</param>
        /// <param name="tileDescriptor">A reference to the tile descriptor in use.</param>
        public MapDescriptor(ILogger logger, IMap map, IProtocolTileDescriptor tileDescriptor)
        {
            logger.ThrowIfNull(nameof(logger));
            map.ThrowIfNull(nameof(map));
            tileDescriptor.ThrowIfNull(nameof(tileDescriptor));

            this.Logger = logger.ForContext<MapDescriptor>();

            this.map = map;
            this.tileDescriptor = tileDescriptor;
        }

        /// <summary>
        /// Gets the reference to the current logger.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the description bytes of the map in behalf of a given player at a given location.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="centerLocation">The center location from which the description is being retrieved.</param>
        /// <returns>A tuple containing the description metadata: a map of string to objects, and the description data: a sequence of bytes representing the description.</returns>
        public (IDictionary<string, object> descriptionMetadata, ReadOnlySequence<byte> descriptionData) DescribeAt(IPlayer player, Location centerLocation)
        {
            player.ThrowIfNull(nameof(player));

            ushort fromX = (ushort)(centerLocation.X - 8);
            ushort fromY = (ushort)(centerLocation.Y - 6);

            sbyte fromZ = (sbyte)(centerLocation.Z > 7 ? centerLocation.Z - 2 : 7);
            sbyte toZ = (sbyte)(centerLocation.Z > 7 ? centerLocation.Z + 2 : 0);

            return this.DescribeWindow(player, fromX, fromY, fromZ, toZ);
        }

        /// <summary>
        /// Gets the description bytes of the map in behalf of a given player for the specified window.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="fromX">The starting X coordinate of the window.</param>
        /// <param name="fromY">The starting Y coordinate of the window.</param>
        /// <param name="fromZ">The starting Z coordinate of the window.</param>
        /// <param name="toZ">The ending Z coordinate of the window.</param>
        /// <param name="windowSizeX">The size of the window in X.</param>
        /// <param name="windowSizeY">The size of the window in Y.</param>
        /// <param name="customOffsetZ">Optional. A custom Z offset value used mainly for partial floor changing windows. Defaults to 0.</param>
        /// <returns>A tuple containing the description metadata: a map of string to objects, and the description data: a sequence of bytes representing the description.</returns>
        public (IDictionary<string, object> descriptionMetadata, ReadOnlySequence<byte> descriptionData) DescribeWindow(IPlayer player, ushort fromX, ushort fromY, sbyte fromZ, sbyte toZ, byte windowSizeX = MapConstants.DefaultWindowSizeX, byte windowSizeY = MapConstants.DefaultWindowSizeY, sbyte customOffsetZ = 0)
        {
            player.ThrowIfNull(nameof(player));

            ushort toX = (ushort)(fromX + windowSizeX);
            ushort toY = (ushort)(fromY + windowSizeY);

            var firstSegment = new MapDescriptionSegment(ReadOnlyMemory<byte>.Empty);

            MapDescriptionSegment lastSegment = firstSegment;

            sbyte stepZ = 1;
            byte currentSkipCount = byte.MaxValue;

            if (fromZ > toZ)
            {
                // we're going up!
                stepZ = -1;
            }

            customOffsetZ = customOffsetZ != 0 ? customOffsetZ : (sbyte)(player.Location.Z - fromZ);

            if (windowSizeX > MapConstants.DefaultWindowSizeX)
            {
                this.Logger.Debug($"{nameof(this.DescribeWindow)} {nameof(windowSizeX)} is over {nameof(MapConstants.DefaultWindowSizeX)} ({MapConstants.DefaultWindowSizeX}).");
            }

            if (windowSizeY > MapConstants.DefaultWindowSizeY)
            {
                this.Logger.Debug($"{nameof(this.DescribeWindow)} {nameof(windowSizeY)} is over {nameof(MapConstants.DefaultWindowSizeY)} ({MapConstants.DefaultWindowSizeY}).");
            }

            var allCreatureIdsToLearn = new List<uint>();
            var allCreatureIdsToForget = new List<uint>();

            for (sbyte currentZ = fromZ; currentZ != toZ + stepZ; currentZ += stepZ)
            {
                var zOffset = fromZ - currentZ + customOffsetZ;

                for (var nx = 0; nx < windowSizeX; nx++)
                {
                    for (var ny = 0; ny < windowSizeY; ny++)
                    {
                        Location targetLocation = new Location
                        {
                            X = (ushort)(fromX + nx + zOffset),
                            Y = (ushort)(fromY + ny + zOffset),
                            Z = currentZ,
                        };

                        var segmentsFromTile = this.tileDescriptor.DescribeTileForPlayer(player, this.map.GetTileAt(targetLocation), out ISet<uint> creatureIdsToLearn, out ISet<uint> creatureIdsToForget);

                        allCreatureIdsToLearn.AddRange(creatureIdsToLearn);
                        allCreatureIdsToForget.AddRange(creatureIdsToForget);

                        // See if we actually have segments to append.
                        if (segmentsFromTile != null && segmentsFromTile.Any())
                        {
                            if (currentSkipCount < byte.MaxValue)
                            {
                                lastSegment = lastSegment.Append(new byte[] { currentSkipCount, byte.MaxValue });
                            }

                            foreach (var segment in segmentsFromTile)
                            {
                                lastSegment.Append(segment);
                                lastSegment = segment;
                            }

                            currentSkipCount = byte.MinValue;
                            continue;
                        }

                        if (++currentSkipCount == byte.MaxValue)
                        {
                            lastSegment = lastSegment.Append(new byte[] { byte.MaxValue, byte.MaxValue });
                        }
                    }
                }
            }

            if (++currentSkipCount < byte.MaxValue)
            {
                lastSegment = lastSegment.Append(new byte[] { currentSkipCount, byte.MaxValue });
            }

            return (
                new Dictionary<string, object>()
                {
                    { IMapDescriptor.CreatureIdsToLearnMetadataKeyName, allCreatureIdsToLearn.ToHashSet() },
                    { IMapDescriptor.CreatureIdsToForgetMetadataKeyName, allCreatureIdsToForget.ToHashSet() },
                },
                new ReadOnlySequence<byte>(firstSegment, 0, lastSegment, lastSegment.Memory.Length));
        }

        /// <summary>
        /// Gets the description bytes of a single tile of the map in behalf of a given player at a given location.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="location">The location from which the description of the tile is being retrieved.</param>
        /// <returns>A tuple containing the description metadata: a map of string to objects, and the description data: a sequence of bytes representing the tile's description.</returns>
        public (IDictionary<string, object> descriptionMetadata, ReadOnlySequence<byte> descriptionData) DescribeTile(IPlayer player, Location location)
        {
            player.ThrowIfNull(nameof(player));

            if (location.Type != LocationType.Map)
            {
                throw new ArgumentException($"Invalid location {location}.", nameof(location));
            }

            var firstSegment = new MapDescriptionSegment(ReadOnlyMemory<byte>.Empty);

            MapDescriptionSegment lastSegment = firstSegment;

            var segmentsFromTile = this.tileDescriptor.DescribeTileForPlayer(player, this.map.GetTileAt(location), out ISet<uint> creatureIdsToLearn, out ISet<uint> creatureIdsToForget);

            // See if we actually have segments to append.
            if (segmentsFromTile != null && segmentsFromTile.Any())
            {
                foreach (var segment in segmentsFromTile)
                {
                    lastSegment.Append(segment);
                    lastSegment = segment;
                }
            }

            return (
                new Dictionary<string, object>()
                {
                    { IMapDescriptor.CreatureIdsToLearnMetadataKeyName, creatureIdsToLearn },
                    { IMapDescriptor.CreatureIdsToForgetMetadataKeyName, creatureIdsToForget },
                },
                new ReadOnlySequence<byte>(firstSegment, 0, lastSegment, lastSegment.Memory.Length));
        }
    }
}
