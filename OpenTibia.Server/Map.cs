// -----------------------------------------------------------------
// <copyright file="Map.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents the map for the game server.
    /// </summary>
    public class Map : IMap
    {
        public static Location NewbieStart = new Location { X = 32097, Y = 32219, Z = 7 };

        public static Location VeteranStart = new Location { X = 32369, Y = 32241, Z = 7 };

        /// <summary>
        /// The highest floor in the map.
        /// </summary>
        private const int HighestFloor = 0;

        /// <summary>
        /// The deepest floor in the map.
        /// </summary>
        private const int DeepestFloor = 15;

        /// <summary>
        /// The total number or floors in the map.
        /// </summary>
        private const int NumberOfFloors = DeepestFloor - HighestFloor;

        /// <summary>
        /// Holds the <see cref="Tile"/>s data based on <see cref="Location"/>.
        /// </summary>
        private readonly ConcurrentDictionary<Location, Tile> tiles;

        ///// <summary>
        ///// Holds the cache of bytes for <see cref="Tile"/> descriptions, queried by <see cref="Location"/>.
        ///// </summary>
        // private readonly ConcurrentDictionary<Location, (DateTimeOffset, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>)> tilesCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="Map"/> class.
        /// </summary>
        /// <param name="mapLoader">The map loader to use to load this map.</param>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        public Map(IMapLoader mapLoader, ICreatureFinder creatureFinder)
        {
            mapLoader.ThrowIfNull(nameof(mapLoader));
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            this.Loader = mapLoader;
            this.CreatureFinder = creatureFinder;

            this.tiles = new ConcurrentDictionary<Location, Tile>();

            // this.tilesCache = new ConcurrentDictionary<Location, (DateTimeOffset, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>)>();
        }

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
        public ReadOnlyMemory<byte> DescribeForPlayer(IPlayer player, Location centerLocation)
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
        public ReadOnlyMemory<byte> DescribeForPlayer(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, byte windowSizeX = IMap.DefaultWindowSizeX, byte windowSizeY = IMap.DefaultWindowSizeY)
        {
            ushort toX = (ushort)(fromX + windowSizeX - 1);
            ushort toY = (ushort)(fromY + windowSizeY - 1);

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
        /// <returns>The description bytes.</returns>
        public ReadOnlyMemory<byte> DescribeForPlayer(IPlayer player, ushort fromX, ushort toX, ushort fromY, ushort toY, sbyte fromZ, sbyte toZ)
        {
            var totalBytes = new List<byte>();

            sbyte stepZ = 1;
            byte currentSkipCount = byte.MaxValue;

            if (fromZ > toZ)
            {
                stepZ = -1; // we're going up!
            }

            byte windowSizeX = (byte)(toX - fromX + 1);
            byte windowSizeY = (byte)(toY - fromY + 1);

            for (sbyte currentZ = fromZ; currentZ != toZ + stepZ; currentZ += stepZ)
            {
                var zOffset = fromZ - currentZ;

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

                        var bytesFromTile = this.DescribeTileForPlayer(player, targetLocation);

                        if (bytesFromTile.Length > 0)
                        {
                            if (currentSkipCount < byte.MaxValue)
                            {
                                totalBytes.Add(currentSkipCount);
                                totalBytes.Add(0xFF);
                            }

                            currentSkipCount = byte.MinValue;

                            totalBytes.AddRange(bytesFromTile.ToArray());

                            continue;
                        }

                        if (++currentSkipCount == byte.MaxValue)
                        {
                            totalBytes.Add(byte.MaxValue);
                            totalBytes.Add(0xFF);
                        }
                    }
                }
            }

            if (currentSkipCount < byte.MaxValue)
            {
                totalBytes.Add(currentSkipCount);
                totalBytes.Add(0xFF);
            }

            return totalBytes.ToArray();
        }

        /// <summary>
        /// Gets the description of a tile as seen by the given <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player for which the tile is being described.</param>
        /// <param name="location">The location of the tile being described.</param>
        /// <returns>The description bytes.</returns>
        public ReadOnlyMemory<byte> DescribeTileForPlayer(IPlayer player, Location location)
        {
            if (!this.GetTileAt(location, out Tile tile))
            {
                return new byte[0];
            }

            var tempBytes = new List<byte>();
            var count = 0;

            // Add ground and top items.
            if (tile.Ground != null)
            {
                tempBytes.AddRange(BitConverter.GetBytes(tile.Ground.Type.ClientId));
                count++;
            }

            foreach (var item in tile.TopItems1)
            {
                if (count == IMap.MaximumNumberOfThingsToDescribePerTile)
                {
                    break;
                }

                tempBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                if (item.IsCumulative)
                {
                    tempBytes.Add(item.Amount);
                }
                else if (item.IsLiquidPool || item.IsLiquidContainer)
                {
                    tempBytes.Add(item.LiquidType);
                }

                count++;
            }

            foreach (var item in tile.TopItems2)
            {
                if (count == IMap.MaximumNumberOfThingsToDescribePerTile)
                {
                    break;
                }

                tempBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                if (item.IsCumulative)
                {
                    tempBytes.Add(item.Amount);
                }
                else if (item.IsLiquidPool || item.IsLiquidContainer)
                {
                    tempBytes.Add(item.LiquidType);
                }

                count++;
            }

            // Add creatures in the tile.
            foreach (var creatureId in tile.CreatureIds)
            {
                if (count == IMap.MaximumNumberOfThingsToDescribePerTile)
                {
                    break;
                }

                var creature = this.CreatureFinder.FindCreatureById(creatureId);

                if (creature == null)
                {
                    continue;
                }

                if (player.KnowsCreatureWithId(creatureId))
                {
                    tempBytes.Add((byte)OutgoingGamePacketType.AddKnownCreature);
                    tempBytes.Add(0x00);
                    tempBytes.AddRange(BitConverter.GetBytes(creatureId));
                }
                else
                {
                    tempBytes.Add((byte)OutgoingGamePacketType.AddUnknownCreature);
                    tempBytes.Add(0x00);
                    tempBytes.AddRange(BitConverter.GetBytes(player.ChooseToRemoveFromKnownSet()));
                    tempBytes.AddRange(BitConverter.GetBytes(creatureId));

                    // TODO: is this the best spot for this ?
                    player.AddKnownCreature(creatureId);

                    var creatureNameBytes = Encoding.Default.GetBytes(creature.Name);
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)creatureNameBytes.Length));
                    tempBytes.AddRange(creatureNameBytes);
                }

                tempBytes.Add((byte)Math.Min(100, creature.Hitpoints * 100 / creature.MaxHitpoints));
                tempBytes.Add(Convert.ToByte(creature.Direction.GetClientSafeDirection()));

                if (player.CanSee(creature))
                {
                    // Add creature outfit
                    tempBytes.AddRange(BitConverter.GetBytes(creature.Outfit.Id));

                    if (creature.Outfit.Id > 0)
                    {
                        tempBytes.Add(creature.Outfit.Head);
                        tempBytes.Add(creature.Outfit.Body);
                        tempBytes.Add(creature.Outfit.Legs);
                        tempBytes.Add(creature.Outfit.Feet);
                    }
                    else
                    {
                        tempBytes.AddRange(BitConverter.GetBytes(creature.Outfit.ItemIdLookAlike));
                    }
                }
                else
                {
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)0));
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)0));
                }

                tempBytes.Add(creature.EmittedLightLevel);
                tempBytes.Add(creature.EmittedLightColor);

                tempBytes.AddRange(BitConverter.GetBytes(creature.Speed));

                tempBytes.Add(creature.Skull);
                tempBytes.Add(creature.Shield);
            }

            foreach (var item in tile.DownItems)
            {
                if (count == IMap.MaximumNumberOfThingsToDescribePerTile)
                {
                    break;
                }

                tempBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                if (item.IsCumulative)
                {
                    tempBytes.Add(item.Amount);
                }
                else if (item.IsLiquidPool || item.IsLiquidContainer)
                {
                    tempBytes.Add(item.LiquidType);
                }

                count++;
            }

            return tempBytes.ToArray();
        }

        /// <summary>
        /// Attempts to get a <see cref="Tile"/> at a given <see cref="Location"/>, if any.
        /// </summary>
        /// <param name="location">The location to get the file from.</param>
        /// <param name="tile">A reference to the <see cref="Tile"/> found, if any.</param>
        /// <returns>A value indicating whether a <see cref="Tile"/> was found, false otherwise.</returns>
        public bool GetTileAt(Location location, out Tile tile)
        {
            if (!this.Loader.HasLoaded(location.X, location.Y, location.Z))
            {
                foreach (var (loc, t) in this.Loader.Load(location.X, location.X, location.Y, location.Y, location.Z, location.Z))
                {
                    this.tiles[loc] = t;
                }
            }

            return this.tiles.TryGetValue(location, out tile);
        }
    }
}
