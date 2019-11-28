// -----------------------------------------------------------------
// <copyright file="ICreatureCreationMetadata.cs" company="2Dudes">
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
    /// <summary>
    /// Interface for creature metadata information.
    /// </summary>
    public interface ICreatureCreationMetadata
    {
        /// <summary>
        /// Gets the article to use for the creature.
        /// </summary>
        string Article { get; }

        /// <summary>
        /// Gets the name to use for the creature.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the max hitpoints to create the creature with.
        /// </summary>
        ushort MaxHitpoints { get; }

        /// <summary>
        /// Gets the max manapoints to create the creature with.
        /// </summary>
        ushort MaxManapoints { get; }

        /// <summary>
        /// Gets the corpse id to give to the creature.
        /// </summary>
        ushort Corpse { get; }
    }
}
