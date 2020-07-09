// -----------------------------------------------------------------
// <copyright file="ICreatureCreationMetadata.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
        string Id { get; }

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
