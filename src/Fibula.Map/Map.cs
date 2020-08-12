// -----------------------------------------------------------------
// <copyright file="Map.cs" company="2Dudes">
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
    using System.Collections.Concurrent;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Delegates;
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
        }

        /// <summary>
        /// Event invoked when a window of coordinates in the map is loaded.
        /// </summary>
        public event WindowLoaded WindowLoaded;

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
        public IMapLoader Loader { get; }

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

                if (minXLoaded < int.MaxValue)
                {
                    this.WindowLoaded?.Invoke(minXLoaded, maxXLoaded, minYLoaded, maxYLoaded, minZLoaded, maxZLoaded);
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
    }
}
