// -----------------------------------------------------------------
// <copyright file="AllBlankMapLoader.cs" company="2Dudes">
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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    public class AllBlankMapLoader : IMapLoader
    {
        public byte PercentageComplete => 0x64;

        private ConcurrentDictionary<Location, ITile> tilesAndLocations;

        private bool preloaded;

        public AllBlankMapLoader(IItemFactory itemFactory)
        {
            this.ItemFactory = itemFactory;

            this.preloaded = false;
            this.tilesAndLocations = new ConcurrentDictionary<Location, ITile>();
        }

        public IItemFactory ItemFactory { get; }

        public bool HasLoaded(int x, int y, sbyte z)
        {
            return this.tilesAndLocations.ContainsKey(new Location() { X = x, Y = y, Z = z });
        }

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

                    var newTuple = (new Location() { X = x, Y = y, Z = fromZ }, new Tile(groundItem));

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
