// -----------------------------------------------------------------
// <copyright file="PlayerCreationMetadata.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents the creation metadata for players.
    /// </summary>
    public class PlayerCreationMetadata : ICreatureCreationMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerCreationMetadata"/> class.
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="name">The name of the player.</param>
        /// <param name="maxHitpoints">The max hitpoints of this player.</param>
        /// <param name="maxManapoints">The max manapoints of this player.</param>
        /// <param name="hitpoints">The hitpoints of this player.</param>
        /// <param name="manapoints">The manapoints of this player.</param>
        /// <param name="corpse">The corpse id of the player.</param>
        public PlayerCreationMetadata(
            string characterId,
            string name,
            ushort maxHitpoints,
            ushort maxManapoints,
            ushort hitpoints = 0,
            ushort manapoints = 0,
            ushort corpse = 0)
        {
            this.CharacterId = characterId;
            this.Name = name;
            this.MaxHitpoints = maxHitpoints;
            this.MaxManapoints = maxManapoints;
            this.Hitpoints = hitpoints > 0 ? hitpoints : maxHitpoints;
            this.Manapoints = manapoints > 0 ? manapoints : maxManapoints;
            this.Corpse = corpse;
        }

        /// <summary>
        /// Gets the id of the character that the player will represent in game.
        /// </summary>
        public string CharacterId { get; }

        /// <summary>
        /// Gets the player name's article.
        /// </summary>
        public string Article { get; }

        /// <summary>
        /// Gets the player's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the player's hitpoints.
        /// </summary>
        public ushort Hitpoints { get; }

        /// <summary>
        /// Gets the player's maximum hitpoints.
        /// </summary>
        public ushort MaxHitpoints { get; }

        /// <summary>
        /// Gets the player's manapoints.
        /// </summary>
        public ushort Manapoints { get; }

        /// <summary>
        /// Gets the player's maximum manapoints.
        /// </summary>
        public ushort MaxManapoints { get; }

        /// <summary>
        /// Gets the player's corpse id.
        /// </summary>
        public ushort Corpse { get; }
    }
}
