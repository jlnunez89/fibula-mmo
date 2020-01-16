// -----------------------------------------------------------------
// <copyright file="AStarPathFinderOptions.cs" company="2Dudes">
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
    /// <summary>
    /// Class that represents options for the <see cref="AStarPathFinder"/>.
    /// </summary>
    public class AStarPathFinderOptions
    {
        /// <summary>
        /// Gets or sets the default maximum steps to perform in a single search.
        /// </summary>
        public int DefaultMaximumSteps { get; set; }
    }
}
