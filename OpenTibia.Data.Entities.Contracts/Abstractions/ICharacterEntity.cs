// -----------------------------------------------------------------
// <copyright file="ICharacterEntity.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Entities.Contracts.Abstractions
{
    using System;

    /// <summary>
    /// Interface for character entities.
    /// </summary>
    public interface ICharacterEntity : IIdentifiableEntity
    {
        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the id of the account to which this character belongs to.
        /// </summary>
        string AccountId { get; }

        /// <summary>
        /// Gets the character's vocation.
        /// </summary>
        string Vocation { get; }

        /// <summary>
        /// Gets the world where the character exists.
        /// </summary>
        string World { get; }

        /// <summary>
        /// Gets the character's level.
        /// </summary>
        short Level { get; }

        /// <summary>
        /// Gets the character's gender.
        /// </summary>
        byte Gender { get; }

        /// <summary>
        /// Gets the character's creation date and time.
        /// </summary>
        DateTimeOffset Creation { get; }

        /// <summary>
        /// Gets the character's last login date and time.
        /// </summary>
        DateTimeOffset LastLogin { get; }

        /// <summary>
        /// Gets a value indicating whether this character is currently online.
        /// </summary>
        bool IsOnline { get; }

        // byte Hideprofile { get; set; }

        // int Playerdelete { get; set; }

        // string Residence { get; set; }

        // string Oldname { get; set; }

        // string Comment { get; set; }

        // string CharIp { get; set; }
    }
}
