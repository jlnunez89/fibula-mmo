// -----------------------------------------------------------------
// <copyright file="AStarSearchContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.PathFinding.AStar
{
    using Fibula.Common.Utilities;
    using Fibula.Common.Utilities.Pathfinding;
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a search context for the AStar pathfinding.
    /// </summary>
    internal class AStarSearchContext : ISearchContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AStarSearchContext"/> class.
        /// </summary>
        /// <param name="searchId">The id of the search in progress.</param>
        /// <param name="forCreature">The creature on behalf of which the search is being performed.</param>
        /// <param name="considerAvoidsAsBlocking">Optional. A value indicating whether to consider the creature's avoid flags as completely blocking. Defaults to true.</param>
        /// <param name="targetDistance">Optional. A value to use for the target distance from the target node.</param>
        public AStarSearchContext(string searchId, ICreature forCreature, bool considerAvoidsAsBlocking = true, int targetDistance = 1)
        {
            searchId.ThrowIfNullOrWhiteSpace(searchId);

            this.SearchId = searchId;
            this.OnBehalfOfCreature = forCreature;
            this.ConsiderAvoidsAsBlocking = considerAvoidsAsBlocking;
            this.TargetDistance = targetDistance;
        }

        /// <summary>
        /// Gets the location of the tile.
        /// </summary>
        public string SearchId { get; }

        /// <summary>
        /// Gets the creature on behalf of which this node calculates movement costs.
        /// </summary>
        public ICreature OnBehalfOfCreature { get; }

        /// <summary>
        /// Gets a value indicating whether to consider the creature's avoid flags as completely blocking.
        /// </summary>
        public bool ConsiderAvoidsAsBlocking { get; }

        /// <summary>
        /// Gets the target distance from the target node.
        /// </summary>
        public int TargetDistance { get; }
    }
}
