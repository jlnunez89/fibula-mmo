// -----------------------------------------------------------------
// <copyright file="TileNodeCreationArguments.cs" company="2Dudes">
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
    using OpenTibia.Common.Utilities;
    using OpenTibia.Common.Utilities.Pathfinding;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents arguments for <see cref="TileNode"/> creation.
    /// </summary>
    internal class TileNodeCreationArguments : INodeCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileNodeCreationArguments"/> class.
        /// </summary>
        /// <param name="location">The location of the tile.</param>
        /// <param name="forCreature">The creature for which this node will calculate movement costs.</param>
        public TileNodeCreationArguments(Location location, ICreature forCreature)
        {
            this.Location = location;
            this.OnBehalfOfCreature = forCreature;
        }

        /// <summary>
        /// Gets the location of the tile.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the creature on behalf of which this node calculates movement costs.
        /// </summary>
        public ICreature OnBehalfOfCreature { get; }
    }
}