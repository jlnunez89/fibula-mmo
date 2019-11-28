// -----------------------------------------------------------------
// <copyright file="ICreatureFinder.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
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
        /// Gets all creatures known to this finder.
        /// </summary>
        /// <returns>A collection of creature instances.</returns>
        IEnumerable<ICreature> FindAllCreatures();
    }
}