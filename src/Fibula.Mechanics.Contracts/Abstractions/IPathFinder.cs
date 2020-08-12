// -----------------------------------------------------------------
// <copyright file="IPathFinder.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities.Pathfinding;
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Interface for a path finder.
    /// </summary>
    public interface IPathFinder
    {
        /// <summary>
        /// Attempts to find a path using the <see cref="AStar"/> implementation between two <see cref="Location"/>s.
        /// </summary>
        /// <param name="startLocation">The start location.</param>
        /// <param name="targetLocation">The target location to find a path to.</param>
        /// <param name="onBehalfOfCreature">Optional. The creature on behalf of which the search is being performed.</param>
        /// <param name="maxStepsCount">Optional. The maximum number of search steps to perform before giving up on finding the target location. A default value must be picked by the implementation or else it may cost too much to search.</param>
        /// <param name="considerAvoidsAsBlocking">Optional. A value indicating whether to consider the creature avoid tastes as blocking in path finding. Defaults to true.</param>
        /// <param name="targetDistance">Optional. The target distance from the target node to shoot for.</param>
        /// <param name="excludeLocations">Optional. Locations to explicitly exclude as a valid goal in the search.</param>
        /// <returns>A tuple consisting of the result of the path search, the end location before returning (even when giving up), and an <see cref="IEnumerable{T}"/> of <see cref="Direction"/>s leading to that end location.</returns>
        public (SearchState result, Location endLocation, IEnumerable<Direction> directions) FindBetween(
            Location startLocation,
            Location targetLocation,
            ICreature onBehalfOfCreature = null,
            int maxStepsCount = default,
            bool considerAvoidsAsBlocking = true,
            int targetDistance = 1,
            params Location[] excludeLocations);
    }
}
