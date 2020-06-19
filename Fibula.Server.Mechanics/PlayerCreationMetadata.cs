// -----------------------------------------------------------------
// <copyright file="PlayerCreationMetadata.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Mechanics
{
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Class that represents the creation metadata for players.
    /// </summary>
    public class PlayerCreationMetadata : IPlayerCreationMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerCreationMetadata"/> class.
        /// </summary>
        /// <param name="client">The client to associate with the player.</param>
        /// <param name="characterId">The id of the character that this player represents.</param>
        /// <param name="name">The name of the player.</param>
        /// <param name="maxHitpoints">The max hitpoints of this player.</param>
        /// <param name="maxManapoints">The max manapoints of this player.</param>
        /// <param name="hitpoints">The hitpoints of this player.</param>
        /// <param name="manapoints">The manapoints of this player.</param>
        /// <param name="corpse">The corpse id of the player.</param>
        public PlayerCreationMetadata(
            IClient client,
            string characterId,
            string name,
            ushort maxHitpoints,
            ushort maxManapoints,
            ushort hitpoints = 0,
            ushort manapoints = 0,
            ushort corpse = 0)
        {
            client.ThrowIfNull(nameof(client));

            this.Client = client;
            this.Identifier = characterId;
            this.Name = name;
            this.MaxHitpoints = maxHitpoints;
            this.MaxManapoints = maxManapoints;
            this.Hitpoints = hitpoints > 0 ? hitpoints : maxHitpoints;
            this.Manapoints = manapoints > 0 ? manapoints : maxManapoints;
            this.Corpse = corpse;
        }

        /// <summary>
        /// Gets the player's id.
        /// </summary>
        public string Identifier { get; }

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

        /// <summary>
        /// Gets the client to associate to the player.
        /// </summary>
        public IClient Client { get; }
    }
}
