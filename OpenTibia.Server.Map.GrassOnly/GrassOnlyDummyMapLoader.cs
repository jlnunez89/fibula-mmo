// -----------------------------------------------------------------
// <copyright file="GrassOnlyDummyMapLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Map.GrassOnly
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Delegates;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Map;

    /// <summary>
    /// Class that represents a dummy map loader that yields all grass tiles.
    /// </summary>
    public class GrassOnlyDummyMapLoader : IMapLoader
    {
        private ConcurrentDictionary<Location, ITile> tilesAndLocations;

        private bool preloaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrassOnlyDummyMapLoader"/> class.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory.</param>
        public GrassOnlyDummyMapLoader(IItemFactory itemFactory)
        {
            this.ItemFactory = itemFactory;

            this.preloaded = false;
            this.tilesAndLocations = new ConcurrentDictionary<Location, ITile>();
        }

        /// <summary>
        /// Event not in use for this loader.
        /// </summary>
        public event WindowLoaded WindowLoaded;

        /// <summary>
        /// Gets the percentage completed loading the map [0, 100].
        /// </summary>
        public byte PercentageComplete => 0x64;

        /// <summary>
        /// Gets the item factory instance.
        /// </summary>
        public IItemFactory ItemFactory { get; }

        /// <summary>
        /// Gets a value indicating whether this loader has previously loaded the given coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <returns>True if the loader has previously loaded the given coordinates, false otherwise.</returns>
        public bool HasLoaded(int x, int y, sbyte z)
        {
            return this.tilesAndLocations.ContainsKey(new Location() { X = x, Y = y, Z = z });
        }

        /// <summary>
        /// Attempts to load all tiles within a 3 dimensional coordinates window.
        /// </summary>
        /// <param name="fromX">The start X coordinate for the load window.</param>
        /// <param name="toX">The end X coordinate for the load window.</param>
        /// <param name="fromY">The start Y coordinate for the load window.</param>
        /// <param name="toY">The end Y coordinate for the load window.</param>
        /// <param name="fromZ">The start Z coordinate for the load window.</param>
        /// <param name="toZ">The end Z coordinate for the load window.</param>
        /// <returns>A collection of ordered pairs containing the <see cref="Location"/> and its corresponding <see cref="ITile"/>.</returns>
        public IEnumerable<(Location Location, ITile Tile)> Load(int fromX, int toX, int fromY, int toY, sbyte fromZ, sbyte toZ)
        {
            if (fromZ != 7)
            {
                return Enumerable.Empty<(Location, ITile)>();
            }

            if (!this.preloaded)
            {
                this.preloaded = true;

                var madeUpCenterLocation = new Location()
                {
                    X = 1000,
                    Y = 1000,
                    Z = 7,
                };

                return this.PreloadBlankMap(madeUpCenterLocation);
            }

            var tuplesAdded = new List<(Location loc, ITile tile)>();

            for (int x = fromX; x <= toX; x++)
            {
                for (int y = fromY; y <= toY; y++)
                {
                    var groundItem = this.ItemFactory.Create(102);

                    var location = new Location() { X = x, Y = y, Z = fromZ };
                    var newTuple = (location, new Tile(location, groundItem));

                    tuplesAdded.Add(newTuple);
                }
            }

            foreach (var (loc, tile) in tuplesAdded)
            {
                this.tilesAndLocations.TryAdd(loc, tile);
            }

            return tuplesAdded;
        }

        private IEnumerable<(Location Location, ITile Tile)> PreloadBlankMap(Location centerLocation)
        {
            return this.Load(centerLocation.X - 20, centerLocation.X + 20, centerLocation.Y - 10, centerLocation.Y + 10, centerLocation.Z, centerLocation.Z);
        }
    }
}
