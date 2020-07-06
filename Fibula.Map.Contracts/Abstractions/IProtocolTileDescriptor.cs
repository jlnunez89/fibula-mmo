// -----------------------------------------------------------------
// <copyright file="IProtocolTileDescriptor.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Map.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Interface for tile descriptors, which are per protocol.
    /// </summary>
    public interface IProtocolTileDescriptor
    {
        /// <summary>
        /// Gets the description segments of a tile as seen by the given <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player for which the tile is being described.</param>
        /// <param name="tile">The tile being described.</param>
        /// <param name="creatureIdsToLearn">A set of ids of creatures to learn if this description is sent.</param>
        /// <param name="creatureIdsToForget">A set of ids of creatures to forget if this description is sent.</param>
        /// <returns>A collection of description segments from the tile.</returns>
        IEnumerable<MapDescriptionSegment> DescribeTileForPlayer(IPlayer player, ITile tile, out ISet<uint> creatureIdsToLearn, out ISet<uint> creatureIdsToForget);
    }
}
