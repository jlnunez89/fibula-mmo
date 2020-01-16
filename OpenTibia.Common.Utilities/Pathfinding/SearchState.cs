// -----------------------------------------------------------------
// <copyright file="SearchState.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Common.Utilities.Pathfinding
{
    /// <summary>
    /// Enumerates the possible A* algorithm states.
    /// </summary>
    public enum SearchState
    {
        /// <summary>
        /// The algorithm is running, searching for the goal.
        /// </summary>
        Searching,

        /// <summary>
        /// The algorithm has found the goal.
        /// </summary>
        GoalFound,

        /// <summary>
        /// The algorithm has failed to find a solution.
        /// </summary>
        Failed,
    }
}
