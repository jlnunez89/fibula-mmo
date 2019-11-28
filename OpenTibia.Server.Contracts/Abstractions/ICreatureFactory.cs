// -----------------------------------------------------------------
// <copyright file="ICreatureFactory.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for an <see cref="ICreature"/> factory.
    /// </summary>
    public interface ICreatureFactory
    {
        /// <summary>
        /// Creates a new implementation instance of <see cref="ICreature"/> depending on the chosen type.
        /// </summary>
        /// <param name="type">The type of creature to create.</param>
        /// <param name="creatureMetadata">The metadata to create the new creature.</param>
        /// <returns>A new instance of the chosen <see cref="ICreature"/> implementation.</returns>
        ICreature Create(CreatureType type, ICreatureCreationMetadata creatureMetadata);
    }
}
