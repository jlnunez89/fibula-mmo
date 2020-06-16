// -----------------------------------------------------------------
// <copyright file="ICreatureCreationMetadata.cs" company="2Dudes">
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
    /// <summary>
    /// Interface for creature creation metadata information.
    /// </summary>
    public interface ICreatureCreationMetadata
    {
        /// <summary>
        /// Gets the identifier to use when creating the creature.
        /// </summary>
        string Identifier { get; }

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
