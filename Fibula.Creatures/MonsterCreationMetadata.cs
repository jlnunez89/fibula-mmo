// -----------------------------------------------------------------
// <copyright file="MonsterCreationMetadata.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures
{
    using System;
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Class that represents the creation metadata for monsters.
    /// </summary>
    public class MonsterCreationMetadata : ICreatureCreationMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterCreationMetadata"/> class.
        /// </summary>
        /// <param name="raceId">The id of the race of monster to create.</param>
        public MonsterCreationMetadata(ushort raceId)
        {
            this.Id = raceId.ToString();
        }

        /// <summary>
        /// Gets the monster's race id as a string.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets nothing. Not supported on this class.
        /// </summary>
        public string Name => throw new NotSupportedException();

        /// <summary>
        /// Gets nothing. Not supported on this class.
        /// </summary>
        public ushort Hitpoints => throw new NotSupportedException();

        /// <summary>
        /// Gets nothing. Not supported on this class.
        /// </summary>
        public ushort MaxHitpoints => throw new NotSupportedException();

        /// <summary>
        /// Gets nothing. Not supported on this class.
        /// </summary>
        public ushort Manapoints => throw new NotSupportedException();

        /// <summary>
        /// Gets nothing. Not supported on this class.
        /// </summary>
        public ushort MaxManapoints => throw new NotSupportedException();

        /// <summary>
        /// Gets nothing. Not supported on this class.
        /// </summary>
        public ushort Corpse => throw new NotSupportedException();
    }
}
