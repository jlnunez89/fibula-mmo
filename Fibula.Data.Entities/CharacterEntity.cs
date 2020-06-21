// -----------------------------------------------------------------
// <copyright file="CharacterEntity.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Entities
{
    using System;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Data.Entities.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a character entity.
    /// </summary>
    public class CharacterEntity : BaseEntity, ICharacterEntity, ICreatureCreationMetadata
    {
        /// <summary>
        /// Gets or sets the character's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the id of the account which this character belongs to.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the character's vocation.
        /// </summary>
        public string Vocation { get; set; }

        /// <summary>
        /// Gets or sets the world where the character exists in.
        /// </summary>
        public string World { get; set; }

        /// <summary>
        /// Gets or sets the character's level.
        /// </summary>
        // TODO: should this one live here? Should it be considered a skill, i.e. "Experience Level"?.
        public short Level { get; set; }

        /// <summary>
        /// Gets or sets the character's chosen gender.
        /// </summary>
        public byte Gender { get; set; }

        /// <summary>
        /// Gets or sets this character's creation date and time.
        /// </summary>
        public DateTimeOffset Creation { get; set; }

        /// <summary>
        /// Gets or sets the last observed date and time that this character successfully loged in.
        /// </summary>
        public DateTimeOffset LastLogin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this character is currently online.
        /// </summary>
        public bool IsOnline { get; set; }

        public ushort MaxHitpoints => 100;

        public ushort MaxManapoints => 0;

        public ushort Corpse => 4240;
    }
}