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

namespace OpenTibia.Server.Factories
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Monsters;

    /// <summary>
    /// Class that represents a factory of creatures.
    /// </summary>
    public class CreatureFactory : ICreatureFactory
    {
        /// <summary>
        /// Stores the catalog of monster types, which is a mapping of the raceId and the type.
        /// </summary>
        private readonly IDictionary<ushort, IMonsterType> monsterTypeCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureFactory"/> class.
        /// </summary>
        /// <param name="monsterLoader">A reference to the monster type loader in use.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        public CreatureFactory(IMonsterTypeLoader monsterLoader, IItemFactory itemFactory)
        {
            monsterLoader.ThrowIfNull(nameof(monsterLoader));
            itemFactory.ThrowIfNull(nameof(itemFactory));

            this.monsterTypeCatalog = monsterLoader.LoadTypes();

            this.ItemFactory = itemFactory;
        }

        /// <summary>
        /// Gets the item factory in use.
        /// </summary>
        public IItemFactory ItemFactory { get; }

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
                            playerMetadata.Identifier,
                            playerMetadata.Name,
                            playerMetadata.MaxHitpoints,
                            playerMetadata.MaxManapoints,
                            playerMetadata.Corpse,
                            playerMetadata.Hitpoints,
                            playerMetadata.Manapoints);
                    }

                    throw new InvalidCastException($"{nameof(creatureMetadata)} must be castable to {nameof(PlayerCreationMetadata)} when {type} is used.");

                case CreatureType.Monster:
                    // Find the actual monster type to init with.
                    var raceId = Convert.ToUInt16(creatureMetadata.Identifier);

                    if (this.monsterTypeCatalog.TryGetValue(raceId, out IMonsterType monsterType))
                    {
                        return new Monster(monsterType, this.ItemFactory);
                    }

                    throw new InvalidOperationException($"{nameof(creatureMetadata)} has an invalid race Id {creatureMetadata.Identifier}. No monster could be created.");
            }

            throw new NotSupportedException($"{nameof(CreatureFactory)} does not support creation of creatures with type {type}.");
        }
    }
}
