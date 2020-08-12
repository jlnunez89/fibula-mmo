// -----------------------------------------------------------------
// <copyright file="AStarPathFinderOptions.cs" company="2Dudes">
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
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class that represents options for the <see cref="AStarPathFinder"/>.
    /// </summary>
    public class AStarPathFinderOptions
    {
        /// <summary>
        /// Gets or sets the default maximum steps to perform in a single search.
        /// </summary>
        [Range(50, 10000, ErrorMessage = "The value for " + nameof(DefaultMaximumSteps) + " must be in the range [50, 10000].")]
        public int DefaultMaximumSteps { get; set; }
    }
}
