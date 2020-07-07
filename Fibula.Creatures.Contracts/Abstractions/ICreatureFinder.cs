// -----------------------------------------------------------------
// <copyright file="ICreatureFinder.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Abstractions
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a creature finder.
    /// </summary>
    public interface ICreatureFinder
    {
        /// <summary>
        /// Looks for a single creature with the id.
        /// </summary>
        /// <param name="creatureId">The creature id for which to look.</param>
        /// <returns>The creature instance, if found, and null otherwise.</returns>
        ICreature FindCreatureById(uint creatureId);

        /// <summary>
        /// Looks for a single player with the id.
        /// </summary>
        /// <param name="playerId">The player id for which to look.</param>
        /// <returns>The player instance, if found, and null otherwise.</returns>
        IPlayer FindPlayerById(uint playerId);

        /// <summary>
        /// Gets all creatures known to this finder.
        /// </summary>
        /// <returns>A collection of creature instances.</returns>
        IEnumerable<ICreature> FindAllCreatures();

        /// <summary>
        /// Gets all players known to this finder.
        /// </summary>
        /// <returns>A collection of player instances.</returns>
        IEnumerable<IPlayer> FindAllPlayers();
    }
}
