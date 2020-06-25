// -----------------------------------------------------------------
// <copyright file="Map.cs" company="2Dudes">
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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Extensions;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Constants;
    using Serilog;

    /// <summary>
    /// Class that represents the map for the game server.
    /// </summary>
    public class Map : IMap
    {
        /// <summary>
        /// Holds the <see cref="ITile"/>s data based on <see cref="Location"/>.
        /// </summary>
        private readonly ConcurrentDictionary<Location, ITile> tiles;

        /// <summary>
        /// Holds the cache of bytes for tile descriptions, queried by <see cref="Location"/>.
        /// </summary>
        private readonly IDictionary<Location, (DateTimeOffset, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, int[])> tilesCache;

        /// <summary>
        /// A lock object for the <see cref="tilesCache"/>.
        /// </summary>
        private readonly object tilesCacheLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="Map"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger to use.</param>
        /// <param name="mapLoader">The map loader to use to load this map.</param>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        public Map(ILogger logger, IMapLoader mapLoader, ICreatureFinder creatureFinder)
        {
            logger.ThrowIfNull(nameof(logger));
            mapLoader.ThrowIfNull(nameof(mapLoader));
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            this.Logger = logger.ForContext<Map>();
            this.Loader = mapLoader;
            this.CreatureFinder = creatureFinder;

            this.tiles = new ConcurrentDictionary<Location, ITile>();

            this.tilesCache = new Dictionary<Location, (DateTimeOffset, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, int[])>();
            this.tilesCacheLock = new object();
        }

        /// <summary>
        /// Gets the reference to the current logger.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the reference to the selected map loader.
        /// </summary>
        private IMapLoader Loader { get; }

        /// <summary>
        /// Gets the description of the map as seen by the given <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player for which the map is being described.</param>
        /// <param name="centerLocation">The center location from which to get the description.</param>
        /// <returns>The description bytes.</returns>
        public ReadOnlySequence<byte> DescribeForPlayer(IPlayer player, Location centerLocation)
        {
            ushort fromX = (ushort)(centerLocation.X - 8);
            ushort fromY = (ushort)(centerLocation.Y - 6);

            return this.DescribeForPlayer(player, fromX, fromY, centerLocation.Z);
        }

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
        public ReadOnlySequence<byte> DescribeForPlayer(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, byte windowSizeX = MapConstants.DefaultWindowSizeX, byte windowSizeY = MapConstants.DefaultWindowSizeY)
        {
            ushort toX = (ushort)(fromX + windowSizeX);
            ushort toY = (ushort)(fromY + windowSizeY);

            sbyte fromZ = (sbyte)(currentZ > 7 ? currentZ - 2 : 7);
            sbyte toZ = (sbyte)(currentZ > 7 ? currentZ + 2 : 0);

            return this.DescribeForPlayer(player, fromX, toX, fromY, toY, fromZ, toZ);
        }

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
        /// <param name="customOffsetZ">Optional. A custom Z offset value used mainly for partial floor changing windows. Defaults to 0.</param>
        /// <returns>The description bytes.</returns>
        public ReadOnlySequence<byte> DescribeForPlayer(IPlayer player, ushort fromX, ushort toX, ushort fromY, ushort toY, sbyte fromZ, sbyte toZ, sbyte customOffsetZ = 0)
        {
            player.ThrowIfNull(nameof(player));

            if (fromX > toX)
            {
                throw new ArgumentException($"{nameof(fromX)}:{fromX} must be less than or equal to {nameof(toX)}:{toX}.");
            }

            if (fromY > toY)
            {
                throw new ArgumentException($"{nameof(fromY)}:{fromY} must be less than or equal to {nameof(toY)}:{toY}.");
            }

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

            byte windowSizeX = (byte)(toX - fromX);
            byte windowSizeY = (byte)(toY - fromY);

            if (windowSizeX > MapConstants.DefaultWindowSizeX)
            {
                this.Logger.Debug($"{nameof(this.DescribeForPlayer)} {nameof(windowSizeX)} is over {nameof(MapConstants.DefaultWindowSizeX)} ({MapConstants.DefaultWindowSizeX}).");
            }

            if (windowSizeY > MapConstants.DefaultWindowSizeY)
            {
                this.Logger.Debug($"{nameof(this.DescribeForPlayer)} {nameof(windowSizeY)} is over {nameof(MapConstants.DefaultWindowSizeY)} ({MapConstants.DefaultWindowSizeY}).");
            }

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

                        var segmentsFromTile = this.DescribeTileForPlayer(player, targetLocation);

                        // See if we actually have segments to append.
                        if (segmentsFromTile != null && segmentsFromTile.Any())
                        {
                            if (currentSkipCount < byte.MaxValue)
                            {
                                lastSegment = lastSegment.Append(new byte[] { currentSkipCount, 0xFF });
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
                            lastSegment = lastSegment.Append(new byte[] { byte.MaxValue, 0xFF });
                        }
                    }
                }
            }

            if (++currentSkipCount < byte.MaxValue)
            {
                lastSegment = lastSegment.Append(new byte[] { currentSkipCount, 0xFF });
            }

            return new ReadOnlySequence<byte>(firstSegment, 0, lastSegment, lastSegment.Memory.Length);
        }

        /// <summary>
        /// Gets the description segment of a tile as seen by the given <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player for which the tile is being described.</param>
        /// <param name="location">The location of the tile being described.</param>
        /// <returns>A collection of description segments from the tile.</returns>
        public IEnumerable<MapDescriptionSegment> DescribeTileForPlayer(IPlayer player, Location location)
        {
            if (!this.GetTileAt(location, out ITile tile))
            {
                return Enumerable.Empty<MapDescriptionSegment>();
            }

            var segments = new List<MapDescriptionSegment>();

            lock (this.tilesCacheLock)
            {
                if (!this.tilesCache.TryGetValue(location, out (DateTimeOffset lastModified, ReadOnlyMemory<byte> preCreatureData, ReadOnlyMemory<byte> postCreatureData, int[] dataPointers) cachedTileData) || cachedTileData.lastModified < tile.LastModified)
                {
                    // This tile's data is not cached or it's cached version is no longer valid, we need to regenerate it.
                    var preCreatureDataBytes = new List<byte>();
                    var postCreatureDataBytes = new List<byte>();
                    var currentPointer = 0;
                    var currentCount = 0;
                    var dataPointers = new int[MapConstants.MaximumNumberOfThingsToDescribePerTile * 2];

                    // Add ground and top items.
                    if (tile.Ground != null)
                    {
                        preCreatureDataBytes.AddRange(BitConverter.GetBytes(tile.Ground.Type.ClientId));

                        dataPointers[currentPointer++] = preCreatureDataBytes.Count;
                        currentCount++;
                    }

                    foreach (var item in tile.StayOnTopItems)
                    {
                        if (currentCount == MapConstants.MaximumNumberOfThingsToDescribePerTile)
                        {
                            break;
                        }

                        preCreatureDataBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                        if (item.IsCumulative)
                        {
                            preCreatureDataBytes.Add(item.Amount);
                        }
                        else if (item.IsLiquidPool || item.IsLiquidContainer)
                        {
                            // HACK: The liquid color calculated here is version specific and thus this will break compatibility with other verisions.
                            // TODO: Figure out a way to plug in the version here, or move away from 'hash-caching' the description in bytes here.
                            preCreatureDataBytes.Add((byte)Find772LiquidColorForLiquidType(item.LiquidType));
                        }

                        dataPointers[currentPointer++] = preCreatureDataBytes.Count;
                        currentCount++;
                    }

                    foreach (var item in tile.StayOnBottomItems)
                    {
                        if (currentCount == MapConstants.MaximumNumberOfThingsToDescribePerTile)
                        {
                            break;
                        }

                        preCreatureDataBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                        if (item.IsCumulative)
                        {
                            preCreatureDataBytes.Add(item.Amount);
                        }
                        else if (item.IsLiquidPool || item.IsLiquidContainer)
                        {
                            // HACK: The liquid color calculated here is version specific and thus this will break compatibility with other verisions.
                            // TODO: Figure out a way to plug in the version here, or move away from 'hash-caching' the description in bytes here.
                            preCreatureDataBytes.Add((byte)Find772LiquidColorForLiquidType(item.LiquidType));
                        }

                        dataPointers[currentPointer++] = preCreatureDataBytes.Count;
                        currentCount++;
                    }

                    // fill up the preCreature pointer positions now by copying the value of the last valid pointer into the remaining first half.
                    var copyFromIndex = Math.Max(0, currentPointer - 1);

                    for (; currentPointer < dataPointers.Length / 2; currentPointer++)
                    {
                        dataPointers[currentPointer] = dataPointers[copyFromIndex];
                    }

                    foreach (var item in tile.Items)
                    {
                        if (currentCount == MapConstants.MaximumNumberOfThingsToDescribePerTile)
                        {
                            break;
                        }

                        postCreatureDataBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                        if (item.IsCumulative)
                        {
                            postCreatureDataBytes.Add(item.Amount);
                        }
                        else if (item.IsLiquidPool || item.IsLiquidContainer)
                        {
                            // HACK: The liquid color calculated here is version specific and thus this will break compatibility with other verisions.
                            // TODO: Figure out a way to plug in the version here, or move away from 'hash-caching' the description in bytes here.
                            postCreatureDataBytes.Add((byte)Find772LiquidColorForLiquidType(item.LiquidType));
                        }

                        dataPointers[currentPointer++] = postCreatureDataBytes.Count;
                        currentCount++;
                    }

                    // And fill up the postCreature pointer positions now by copying the value of the last valid pointer into the remaining second half.
                    copyFromIndex = Math.Max(dataPointers.Length / 2, currentPointer - 1);

                    for (; currentPointer < dataPointers.Length; currentPointer++)
                    {
                        dataPointers[currentPointer] = dataPointers[copyFromIndex];
                    }

                    cachedTileData = (tile.LastModified, preCreatureDataBytes.ToArray(), postCreatureDataBytes.ToArray(), dataPointers);

                    this.tilesCache[location] = cachedTileData;

                    // this.Logger.Verbose($"Regenerated description for tile at {location}.");
                }

                // Add a slice of the bytes, using the pointer that corresponds to the location in the memory of the number of items to describe.
                segments.Add(new MapDescriptionSegment(cachedTileData.preCreatureData.Slice(0, cachedTileData.dataPointers[Math.Max(MapConstants.MaximumNumberOfThingsToDescribePerTile - 1 - tile.CreatureCount, 0)])));

                // TODO: The creatures part is more dynamic, figure out how/if we can cache it.
                // HACK: The order of bytes and packet identifiers here is version specific and thus this will break compatibility with other verisions.
                // Add creatures in the tile.
                if (tile.CreatureIds.Any())
                {
                    var creatureBytes = new List<byte>();

                    foreach (var creatureId in tile.CreatureIds)
                    {
                        var creature = this.CreatureFinder.FindCreatureById(creatureId);

                        if (creature == null)
                        {
                            continue;
                        }

                        if (player.KnowsCreatureWithId(creatureId))
                        {
                            creatureBytes.Add((byte)OutgoingGamePacketType.AddKnownCreature);
                            creatureBytes.Add(0x00);
                            creatureBytes.AddRange(BitConverter.GetBytes(creatureId));
                        }
                        else
                        {
                            creatureBytes.Add((byte)OutgoingGamePacketType.AddUnknownCreature);
                            creatureBytes.Add(0x00);
                            creatureBytes.AddRange(BitConverter.GetBytes(player.ChooseCreatureToRemoveFromKnownSet()));
                            creatureBytes.AddRange(BitConverter.GetBytes(creatureId));

                            // TODO: is this the best spot for this ?
                            player.AddKnownCreature(creatureId);

                            var creatureNameBytes = Encoding.Default.GetBytes(creature.Name);
                            creatureBytes.AddRange(BitConverter.GetBytes((ushort)creatureNameBytes.Length));
                            creatureBytes.AddRange(creatureNameBytes);
                        }

                        creatureBytes.Add((byte)Math.Min(100, creature.Hitpoints * 100 / creature.MaxHitpoints));
                        creatureBytes.Add(Convert.ToByte(creature.Direction.GetClientSafeDirection()));

                        if (player.CanSee(creature))
                        {
                            // Add creature outfit
                            creatureBytes.AddRange(BitConverter.GetBytes(creature.Outfit.Id));

                            if (creature.Outfit.Id > 0)
                            {
                                creatureBytes.Add(creature.Outfit.Head);
                                creatureBytes.Add(creature.Outfit.Body);
                                creatureBytes.Add(creature.Outfit.Legs);
                                creatureBytes.Add(creature.Outfit.Feet);
                            }
                            else
                            {
                                creatureBytes.AddRange(BitConverter.GetBytes(creature.Outfit.ItemIdLookAlike));
                            }
                        }
                        else
                        {
                            creatureBytes.AddRange(BitConverter.GetBytes((ushort)0));
                            creatureBytes.AddRange(BitConverter.GetBytes((ushort)0));
                        }

                        creatureBytes.Add(creature.EmittedLightLevel);
                        creatureBytes.Add(creature.EmittedLightColor);

                        creatureBytes.AddRange(BitConverter.GetBytes(creature.Speed));

                        creatureBytes.Add(0x00); // Skull
                        creatureBytes.Add(0x00); // Shield
                    }

                    segments.Add(new MapDescriptionSegment(creatureBytes.ToArray()));
                }

                // Add a slice of the bytes, using the pointer that corresponds to the location in the memory of the number of items to describe.
                segments.Add(new MapDescriptionSegment(cachedTileData.postCreatureData.Slice(0, cachedTileData.dataPointers[(cachedTileData.dataPointers.Length / 2) + Math.Max(MapConstants.MaximumNumberOfThingsToDescribePerTile - 1 - tile.CreatureCount, 0)])));

                return segments;
            }
        }

        /// <summary>
        /// Attempts to get a <see cref="ITile"/> at a given <see cref="Location"/>, if any.
        /// </summary>
        /// <param name="location">The location to get the file from.</param>
        /// <param name="tile">A reference to the <see cref="ITile"/> found, if any.</param>
        /// <param name="loadAsNeeded">Optional. A value indicating whether to attempt to load tiles if the loader hasn't loaded them yet.</param>
        /// <returns>A value indicating whether a <see cref="ITile"/> was found, false otherwise.</returns>
        public bool GetTileAt(Location location, out ITile tile, bool loadAsNeeded = true)
        {
            if (loadAsNeeded && !this.Loader.HasLoaded(location.X, location.Y, location.Z))
            {
                int minXLoaded = int.MaxValue;
                int maxXLoaded = int.MinValue;
                int minYLoaded = int.MaxValue;
                int maxYLoaded = int.MinValue;
                sbyte minZLoaded = sbyte.MaxValue;
                sbyte maxZLoaded = sbyte.MinValue;

                foreach (var (loc, t) in this.Loader.Load(location.X, location.X, location.Y, location.Y, location.Z, location.Z))
                {
                    this.tiles[loc] = t;

                    minXLoaded = Math.Min(loc.X, minXLoaded);
                    maxXLoaded = Math.Max(loc.X, maxXLoaded);

                    minYLoaded = Math.Min(loc.Y, minYLoaded);
                    maxYLoaded = Math.Max(loc.Y, maxYLoaded);

                    minZLoaded = Math.Min(loc.Z, minZLoaded);
                    maxZLoaded = Math.Max(loc.Z, maxZLoaded);
                }
            }

            return this.tiles.TryGetValue(location, out tile);
        }

        /// <summary>
        /// Attempts to get a <see cref="ITile"/> at a given <see cref="Location"/>, if any.
        /// </summary>
        /// <param name="location">The location to get the file from.</param>
        /// <returns>A reference to the <see cref="ITile"/> found, if any.</returns>
        public ITile GetTileAt(Location location)
        {
            return this.GetTileAt(location, out ITile tile) ? tile : null;
        }

        /// <summary>
        /// Finds the <see cref="LiquidColor"/> of the <see cref="LiquidType"/> to send to the 7.72 client.
        /// </summary>
        /// <param name="liquidType">The type of liquid.</param>
        /// <returns>The color supported by the 7.72 client.</returns>
        /// <remarks>This whole method is a HACK and should be removed.</remarks>
        private static LiquidColor Find772LiquidColorForLiquidType(LiquidType liquidType)
        {
            switch (liquidType)
            {
                default:
                case LiquidType.None:
                    return LiquidColor.None;
                case LiquidType.Water:
                    return LiquidColor.Blue;
                case LiquidType.Wine:
                case LiquidType.ManaFluid:
                    return LiquidColor.Purple;
                case LiquidType.Beer:
                case LiquidType.Mud:
                case LiquidType.Oil:
                    return LiquidColor.Brown;
                case LiquidType.Blood:
                case LiquidType.LifeFluid:
                    return LiquidColor.Red;
                case LiquidType.Slime:
                case LiquidType.Lemonade:
                    return LiquidColor.Green;
                case LiquidType.Urine:
                    return LiquidColor.Yellow;
                case LiquidType.Milk:
                    return LiquidColor.White;
            }
        }
    }
}
