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
        /// <param name="nodeFactory">A reference to the node factory in use.</param>
        /// <param name="tileAccessor">A refernce to the file accessor.</param>
        /// <param name="pathfinderOptions">The options for this pathfinder.</param>
        public AStarPathFinder(INodeFactory nodeFactory, ITileAccessor tileAccessor, IOptions<AStarPathFinderOptions> pathfinderOptions)
        {
            nodeFactory.ThrowIfNull(nameof(nodeFactory));
            tileAccessor.ThrowIfNull(nameof(tileAccessor));
            pathfinderOptions?.Value.ThrowIfNull(nameof(pathfinderOptions));

            this.NodeFactory = nodeFactory;
            this.TileAccessor = tileAccessor;
            this.Options = pathfinderOptions.Value;
        }

        /// <summary>
        /// Gets a reference to the node factory in use.
        /// </summary>
        public INodeFactory NodeFactory { get; }

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
        /// <param name="considerAvoidsAsBlock">Optional. A value indicating whether to consider the creature avoid tastes as blocking in path finding. Defaults to true.</param>
        /// <param name="targetDistance">Optional. The target distance from the target node to shoot for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Direction"/>s leading to the end location. The <paramref name="endLocation"/> and <paramref name="targetLocation"/> may or may not be the same.</returns>
        public IEnumerable<Direction> FindBetween(Location startLocation, Location targetLocation, out Location endLocation, int maxStepsCount = default, ICreature onBehalfOfCreature = null, bool considerAvoidsAsBlock = true, int targetDistance = 1)
        {
            endLocation = startLocation;
            maxStepsCount = maxStepsCount == default ? this.Options.DefaultMaximumSteps : maxStepsCount;

            var searchContext = new AStarSearchContext(
                Guid.NewGuid().ToString(),
                onBehalfOfCreature,
                considerAvoidsAsBlock,
                targetDistance);

            var startNode = this.NodeFactory.Create(searchContext, new TileNodeCreationArguments(startLocation));

            try
            {
                var targetNode = this.NodeFactory.Create(searchContext, new TileNodeCreationArguments(targetLocation));

                if (startLocation == targetLocation || startNode == null || targetNode == null)
                {
                    return Enumerable.Empty<Direction>();
                }

                var dirList = new List<Direction>();

                var algo = new AStar(this.NodeFactory, startNode, targetNode, maxStepsCount);

                if (algo.Run() == SearchState.Failed)
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
            finally
            {
                this.NodeFactory.OnSearchCompleted(searchContext.SearchId);
            }
        }
    }
}
