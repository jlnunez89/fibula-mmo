// -----------------------------------------------------------------
// <copyright file="SearchState.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Utilities.Pathfinding
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
