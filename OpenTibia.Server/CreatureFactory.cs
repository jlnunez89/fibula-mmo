// -----------------------------------------------------------------
// <copyright file="CreatureFactory.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a factory of creatures.
    /// </summary>
    public class CreatureFactory : ICreatureFactory
    {
        /// <summary>
        /// Creates a new implementation instance of <see cref="ICreature"/> depending on the chosen type.
        /// </summary>
        /// <param name="type">The type of creature to create.</param>
        /// <param name="creatureMetadata">The metadata to create the new creature.</param>
        /// <returns>A new instance of the chosen <see cref="ICreature"/> implementation.</returns>
        public ICreature Create(CreatureType type, ICreatureCreationMetadata creatureMetadata)
        {
            switch (type)
            {
                case CreatureType.NonPlayerCharacter:

                // if (creatureMetadata is NonPlayerCharacterMetadata npcMetadata)
                // {
                //    return new NonPlayerCharacter(
                //        npcMetadata.CreatureId,
                //        npcMetadata.Name,
                //        npcMetadata.MaxHitpoints,
                //        npcMetadata.MaxManapoints,
                //        npcMetadata.Corpse,
                //        npcMetadata.Hitpoints,
                //        npcMetadata.Manapoints);
                // }

                // throw new InvalidCastException($"{nameof(creatureMetadata)} must be castable to {nameof(NonPlayerCharacterMetadata)} when {type} is used.");

                case CreatureType.Player:

                    if (creatureMetadata is PlayerCreationMetadata playerMetadata)
                    {
                        return new Player(
                            playerMetadata.CharacterId,
                            playerMetadata.Name,
                            playerMetadata.MaxHitpoints,
                            playerMetadata.MaxManapoints,
                            playerMetadata.Corpse,
                            playerMetadata.Hitpoints,
                            playerMetadata.Manapoints);
                    }

                    throw new InvalidCastException($"{nameof(creatureMetadata)} must be castable to {nameof(PlayerCreationMetadata)} when {type} is used.");

                // case CreatureType.Monster:

                    // if (creatureMetadata is MonsterCreationMetadata monsterMetadata)
                    // {
                    //    return new Monster(gameInstance, monsterMetadata.Type);
                    // }

                    // throw new InvalidCastException($"{nameof(creatureMetadata)} must be castable to {nameof(MonsterCreationMetadata)} when {type} is used.");
            }

            throw new NotSupportedException($"{nameof(CreatureFactory)} does not support creation of creatures with type {type}.");
        }
    }
}
