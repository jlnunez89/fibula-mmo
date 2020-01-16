// -----------------------------------------------------------------
// <copyright file="AStarPathFinder.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.PathFinding.AStar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Options;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Common.Utilities.Pathfinding;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents a path finder that implements the A* algorithm to find a path bewteen two <see cref="Location"/>s.
    /// </summary>
    public class AStarPathFinder : IPathFinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AStarPathFinder"/> class.
        /// </summary>
        /// <param name="tileAccessor">A refernce to the file accessor.</param>
        /// <param name="pathfinderOptions">The options for this pathfinder.</param>
        public AStarPathFinder(ITileAccessor tileAccessor, IOptions<AStarPathFinderOptions> pathfinderOptions)
        {
            tileAccessor.ThrowIfNull(nameof(tileAccessor));
            pathfinderOptions?.Value.ThrowIfNull(nameof(pathfinderOptions));

            this.TileAccessor = tileAccessor;
            this.Options = pathfinderOptions.Value;
        }

        /// <summary>
        /// Gets the tile accessor in use.
        /// </summary>
        public ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Gets the options to use for this path finder.
        /// </summary>
        public AStarPathFinderOptions Options { get; }

        /// <summary>
        /// Attempts to find a path using the <see cref="AStar"/> implementation between two <see cref="Location"/>s.
        /// </summary>
        /// <param name="startLocation">The start location.</param>
        /// <param name="targetLocation">The target location to find a path to.</param>
        /// <param name="endLocation">The last searched location before returning.</param>
        /// <param name="maxStepsCount">Optional. The maximum number of search steps to perform before giving up on finding the target location. Default is 100.</param>
        /// <param name="onBehalfOfCreature">Optional. The creature on behalf of which the search is being performed.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Direction"/>s leading to the end location. The <paramref name="endLocation"/> and <paramref name="targetLocation"/> may or may not be the same.</returns>
        public IEnumerable<Direction> FindBetween(Location startLocation, Location targetLocation, out Location endLocation, int maxStepsCount = default, ICreature onBehalfOfCreature = null)
        {
            endLocation = startLocation;
            maxStepsCount = maxStepsCount == default ? this.Options.DefaultMaximumSteps : maxStepsCount;

            if (!this.TileAccessor.GetTileAt(startLocation, out ITile fromTile) || !this.TileAccessor.GetTileAt(targetLocation, out ITile toTile))
            {
                return Enumerable.Empty<Direction>();
            }

            var dirList = new List<Direction>();
            var searchId = Guid.NewGuid().ToString();
            var algo = new AStar(new TileNode(searchId, this.TileAccessor, fromTile, onBehalfOfCreature), new TileNode(searchId, this.TileAccessor, toTile, onBehalfOfCreature), maxStepsCount);
            var result = algo.Run();

            if (result == SearchState.Failed)
            {
                var lastTile = algo.GetLastPath()?.LastOrDefault() as TileNode;

                if (lastTile?.Tile != null)
                {
                    endLocation = lastTile.Tile.Location;
                }

                return dirList;
            }

            var lastLoc = startLocation;

            foreach (var node in algo.GetLastPath().Cast<TileNode>().Skip(1))
            {
                var newDir = lastLoc.DirectionTo(node.Tile.Location, true);

                dirList.Add(newDir);

                lastLoc = node.Tile.Location;
            }

            endLocation = lastLoc;

            return dirList;
        }
    }
}
