// -----------------------------------------------------------------
// <copyright file="MonsterCreationMetadata.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using OpenTibia.Server.Contracts.Abstractions;

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
            this.Identifier = raceId.ToString();
        }

        /// <summary>
        /// Gets the monster's race id as a string.
        /// </summary>
        public string Identifier { get; }

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
