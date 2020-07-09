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
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Interface for a path finder.
    /// </summary>
    public interface IPathFinder
    {
        /// <summary>
        /// Attempts to find a path between two <see cref="Location"/>s.
        /// </summary>
        /// <param name="startLocation">The start location.</param>
        /// <param name="targetLocation">The target location to find a path to.</param>
        /// <param name="endLocation">The last searched location before returning.</param>
        /// <param name="onBehalfOfCreature">Optional. The creature on behalf of which the search is being performed.</param>
        /// <param name="maxStepsCount">Optional. The maximum number of search steps to perform before giving up on finding the target location. Default is 100.</param>
        /// <param name="considerAvoidsAsBlocking">Optional. A value indicating whether to consider the creature avoid tastes as blocking in path finding. Defaults to true.</param>
        /// <param name="targetDistance">Optional. The target distance from the target node to shoot for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Direction"/>s leading to the end location. The <paramref name="endLocation"/> and <paramref name="targetLocation"/> may or may not be the same.</returns>
        IEnumerable<Direction> FindBetween(Location startLocation, Location targetLocation, out Location endLocation, ICreature onBehalfOfCreature = null, int maxStepsCount = 100, bool considerAvoidsAsBlocking = true, int targetDistance = 1);
    }
}
