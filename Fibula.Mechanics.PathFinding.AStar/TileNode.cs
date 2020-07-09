// -----------------------------------------------------------------
// <copyright file="TileNode.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.PathFinding.AStar
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Common.Utilities.Pathfinding;
    using Fibula.Map.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a tile node.
    /// </summary>
    internal class TileNode : INode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileNode"/> class.
        /// </summary>
        /// <param name="searchContext">A reference to the context of the search this node takes place in.</param>
        /// <param name="tile">The tile to reference in this node.</param>
        public TileNode(ISearchContext searchContext, ITile tile)
        {
            searchContext.ThrowIfNull(nameof(searchContext));
            tile.ThrowIfNull(nameof(tile));

            if (!(searchContext is AStarSearchContext aStarSearchContext))
            {
                throw new ArgumentException($"{nameof(searchContext)} must be of type {nameof(AStarSearchContext)} for this node type.");
            }

            this.SearchContext = aStarSearchContext;
            this.Tile = tile;
        }

        /// <summary>
        /// Gets the context of the search this node belongs in.
        /// </summary>
        public AStarSearchContext SearchContext { get; }

        /// <summary>
        /// Gets the tile referenced by this node.
        /// </summary>
        public ITile Tile { get; }

        /// <summary>
        /// Gets the total cost for this node.
        /// f = g + h
        /// TotalCost = MovementCost + EstimatedCost.
        /// </summary>
        public int TotalCost => this.MovementCost + this.EstimatedCost;

        /// <summary>
        /// Gets the movement cost for this node.
        /// This is the movement cost from this node to the starting node, or g.
        /// </summary>
        public int MovementCost { get; private set; }

        /// <summary>
        /// Gets the estimated cost for this node.
        /// This is the heuristic from this node to the goal node, or h.
        /// </summary>
        public int EstimatedCost { get; private set; }

        /// <summary>
        /// Gets or sets the parent node to this node.
        /// </summary>
        public INode Parent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this node is part of the open list.
        /// </summary>
        public bool ShouldBeVisited { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this node is part of the closed list.
        /// </summary>
        public bool HasBeenVisited { get; set; }

        /// <summary>
        /// Gets this node's adjacent nodes.
        /// </summary>
        /// <param name="nodeFactory">A reference to the factory to create new nodes.</param>
        /// <returns>The collection of adjacent nodes.</returns>
        public IEnumerable<INode> FindAdjacent(INodeFactory nodeFactory)
        {
            var adjacent = new List<INode>();

            var currentLoc = this.Tile.Location;

            var rng = new Random();
            var offsets = new List<Location>();

            // look at adjacent tiles.
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    // skip the current tile.
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    offsets.Insert(rng.Next(offsets.Count), new Location { X = dx, Y = dy, Z = 0 });
                }
            }

            foreach (var locOffset in offsets)
            {
                if (nodeFactory.Create(this.SearchContext, new TileNodeCreationArguments(currentLoc + locOffset)) is TileNode tileNode && !tileNode.Tile.IsPathBlocking())
                {
                    adjacent.Add(tileNode);
                }
            }

            return adjacent;
        }

        /// <summary>
        /// Sets the movement cost for the current node, or g.
        /// </summary>
        /// <param name="parent">Parent node, for access to the parents movement cost.</param>
        public void SetMovementCost(INode parent)
        {
            var parentNode = parent as TileNode;

            if (this.Tile == null || parentNode?.Tile == null)
            {
                return;
            }

            var locationDiff = this.Tile.Location - parentNode.Tile.Location;
            var isDiagonal = Math.Min(Math.Abs(locationDiff.X), 1) + Math.Min(Math.Abs(locationDiff.Y), 1) == 2;

            // TODO: check OnBehalfOfCreature avoids anything over this tile by adding weight here.
            var newCost = parent.MovementCost + (isDiagonal ? 3 : 1);

            this.MovementCost = this.MovementCost > 0 ? Math.Min(this.MovementCost, newCost) : newCost;
        }

        /// <summary>
        /// Sets the estimated cost for the current node, or h.
        /// </summary>
        /// <param name="goal">Goal node, for acces to the goals position.</param>
        public void SetEstimatedCost(INode goal)
        {
            var goalNode = goal as TileNode;

            if (this.Tile == null || goalNode?.Tile == null)
            {
                return;
            }

            var locationDiff = this.Tile.Location - goalNode.Tile.Location;

            this.EstimatedCost = Math.Abs(locationDiff.X) + Math.Abs(locationDiff.Y);
        }

        /// <summary>
        /// Returns true if this node is the goal, false if it is not the goal.
        /// </summary>
        /// <param name="goal">The goal node to compare this node against.</param>
        /// <returns>True if this node is the goal, false if it s not the goal.</returns>
        public bool IsGoal(INode goal)
        {
            var goalNode = goal as TileNode;

            if (goalNode?.Tile == null)
            {
                // true to stop ASAP.
                return true;
            }

            var locationDiff = this.Tile.Location - goalNode.Tile.Location;

            return locationDiff.Z == 0 && locationDiff.MaxValueIn2D <= this.SearchContext.TargetDistance;
        }
    }
}
