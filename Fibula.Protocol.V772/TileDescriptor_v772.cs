// -----------------------------------------------------------------
// <copyright file="TileDescriptor_v772.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Protocol.V772
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
    /// Class that represents a tile descriptor for protocol 7.72.
    /// </summary>
    public class TileDescriptor_v772 : IProtocolTileDescriptor
    {
        /// <summary>
        /// Holds the cache of bytes for tile descriptions, queried by <see cref="Location"/>.
        /// </summary>
        private readonly IDictionary<Location, (DateTimeOffset, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, int[])> tileDescriptionCache;

        /// <summary>
        /// A lock object for the <see cref="tileDescriptionCache"/>.
        /// </summary>
        private readonly object tilesCacheLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileDescriptor_v772"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        public TileDescriptor_v772(ILogger logger, ICreatureFinder creatureFinder)
        {
            logger.ThrowIfNull(nameof(logger));
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            this.tileDescriptionCache = new Dictionary<Location, (DateTimeOffset, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, int[])>();
            this.tilesCacheLock = new object();

            this.Logger = logger.ForContext<TileDescriptor_v772>();
            this.CreatureFinder = creatureFinder;
        }

        /// <summary>
        /// Gets the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the creature finder in use.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the description segments of a tile as seen by the given <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player for which the tile is being described.</param>
        /// <param name="tile">The tile being described.</param>
        /// <param name="creatureIdsToLearn">A set of ids of creatures to learn if this description is sent.</param>
        /// <param name="creatureIdsToForget">A set of ids of creatures to forget if this description is sent.</param>
        /// <returns>A collection of description segments from the tile.</returns>
        public IEnumerable<MapDescriptionSegment> DescribeTileForPlayer(IPlayer player, ITile tile, out ISet<uint> creatureIdsToLearn, out ISet<uint> creatureIdsToForget)
        {
            player.ThrowIfNull(nameof(player));

            creatureIdsToLearn = new HashSet<uint>();
            creatureIdsToForget = new HashSet<uint>();

            if (tile == null)
            {
                return Enumerable.Empty<MapDescriptionSegment>();
            }

            var segments = new List<MapDescriptionSegment>();

            lock (this.tilesCacheLock)
            {
                if (!this.tileDescriptionCache.TryGetValue(tile.Location, out (DateTimeOffset lastModified, ReadOnlyMemory<byte> preCreatureData, ReadOnlyMemory<byte> postCreatureData, int[] dataPointers) cachedTileData) || cachedTileData.lastModified < tile.LastModified)
                {
                    // This tile's data is not cached or it's cached version is no longer valid, we need to regenerate it.
                    this.RegenerateTileCachedDescription(tile);

                    cachedTileData = this.tileDescriptionCache[tile.Location];
                }

                // Add a slice of the bytes, using the pointer that corresponds to the location in the memory of the number of items to describe.
                segments.Add(new MapDescriptionSegment(cachedTileData.preCreatureData.Slice(0, cachedTileData.dataPointers[Math.Max(MapConstants.MaximumNumberOfThingsToDescribePerTile - 1 - tile.Creatures.Count(), 0)])));

                // TODO: The creatures part is more dynamic, figure out how/if we can cache it.
                // Add creatures in the tile.
                if (tile.Creatures.Any())
                {
                    var creatureBytes = new List<byte>();

                    foreach (var creature in tile.Creatures)
                    {
                        if (player.Client.KnowsCreatureWithId(creature.Id))
                        {
                            creatureBytes.Add((byte)OutgoingGamePacketType.AddKnownCreature);
                            creatureBytes.Add(0x00);
                            creatureBytes.AddRange(BitConverter.GetBytes(creature.Id));
                        }
                        else
                        {
                            var creatureIdToForget = player.Client.ChooseCreatureToRemoveFromKnownSet(creatureIdsToForget.Count);

                            creatureBytes.Add((byte)OutgoingGamePacketType.AddUnknownCreature);
                            creatureBytes.Add(0x00);
                            creatureBytes.AddRange(BitConverter.GetBytes(creatureIdToForget));
                            creatureBytes.AddRange(BitConverter.GetBytes(creature.Id));

                            if (creatureIdToForget > uint.MinValue)
                            {
                                creatureIdsToForget.Add(creatureIdToForget);
                            }

                            creatureIdsToLearn.Add(creature.Id);

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
                segments.Add(new MapDescriptionSegment(cachedTileData.postCreatureData.Slice(0, cachedTileData.dataPointers[(cachedTileData.dataPointers.Length / 2) + Math.Max(MapConstants.MaximumNumberOfThingsToDescribePerTile - 1 - tile.Creatures.Count(), 0)])));

                return segments;
            }
        }

        private void RegenerateTileCachedDescription(ITile tile)
        {
            // TODO: null (or removing) tiles are not supported.
            tile.ThrowIfNull(nameof(tile));

            var preCreatureDataBytes = new List<byte>();
            var postCreatureDataBytes = new List<byte>();
            var currentPointer = 0;
            var currentCount = 0;
            var dataPointers = new int[MapConstants.MaximumNumberOfThingsToDescribePerTile * 2];

            /*
            // Add ground and top items.
            if (tile.Ground != null)
            {
                preCreatureDataBytes.AddRange(BitConverter.GetBytes(tile.Ground.Type.ClientId));

                dataPointers[currentPointer++] = preCreatureDataBytes.Count;
                currentCount++;
            }
            */

            var (fixedItems, items) = tile.GetItemsToDescribeByPriority();

            foreach (var item in fixedItems)
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
                    preCreatureDataBytes.Add((byte)item.LiquidType.ToLiquidColor());
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

            foreach (var item in items)
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
                    postCreatureDataBytes.Add((byte)item.LiquidType.ToLiquidColor());
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

            this.tileDescriptionCache[tile.Location] = (tile.LastModified, preCreatureDataBytes.ToArray(), postCreatureDataBytes.ToArray(), dataPointers);

            this.Logger.Verbose($"Regenerated description for tile at {tile.Location}.");
        }
    }
}
