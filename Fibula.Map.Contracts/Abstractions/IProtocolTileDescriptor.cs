// -----------------------------------------------------------------
// <copyright file="IProtocolTileDescriptor.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
