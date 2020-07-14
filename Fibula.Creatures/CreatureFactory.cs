// -----------------------------------------------------------------
// <copyright file="CreatureFactory.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Items.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a factory of creatures.
    /// </summary>
    public class CreatureFactory : ICreatureFactory
    {
        /// <summary>
        /// Stores the map between the monster race ids and the actual monster types.
        /// </summary>
        private readonly IDictionary<ushort, IMonsterType> monsterTypeCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureFactory"/> class.
        /// </summary>
        /// <param name="monsterTypeLoader">A reference to the monster type loader in use.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        public CreatureFactory(IMonsterTypeLoader monsterTypeLoader, IItemFactory itemFactory)
        {
            monsterTypeLoader.ThrowIfNull(nameof(monsterTypeLoader));
            itemFactory.ThrowIfNull(nameof(itemFactory));

            this.monsterTypeCatalog = monsterTypeLoader.LoadTypes();

            this.ItemFactory = itemFactory;
        }

        /// <summary>
        /// Gets the item factory in use.
        /// </summary>
        public IItemFactory ItemFactory { get; }

        /// <summary>
        /// Creates a new <see cref="IThing"/>.
        /// </summary>
        /// <param name="creationArguments">The arguments for the <see cref="IThing"/> creation.</param>
        /// <returns>A new instance of the <see cref="IThing"/>.</returns>
        public IThing Create(IThingCreationArguments creationArguments)
        {
            return this.CreateCreature(creationArguments);
        }

        /// <summary>
        /// Creates a new implementation instance of <see cref="ICreature"/> depending on the chosen type.
        /// </summary>
        /// <param name="creationArguments">The arguments for the <see cref="IThing"/> creation.</param>
        /// <returns>A new instance of the chosen <see cref="ICreature"/> implementation.</returns>
        public ICreature CreateCreature(IThingCreationArguments creationArguments)
        {
            if (!(creationArguments is CreatureCreationArguments creatureCreationArguments))
            {
                throw new ArgumentException($"Invalid type of arguments '{creationArguments.GetType().Name}' supplied, expected {nameof(CreatureCreationArguments)}", nameof(creationArguments));
            }

            switch (creatureCreationArguments.Type)
            {
                // TODO: suppport other types
                // case CreatureType.NonPlayerCharacter:
                case CreatureType.Monster:
                    if (creatureCreationArguments.Metadata == null)
                    {
                        throw new ArgumentException("Invalid metadata in creation arguments for a monster.", nameof(creatureCreationArguments));
                    }

                    if (!this.monsterTypeCatalog.TryGetValue(Convert.ToUInt16(creatureCreationArguments.Metadata.Id), out IMonsterType monsterType))
                    {
                        throw new ArgumentException($"Unknown monster with Id {creatureCreationArguments.Metadata.Id} in creation arguments for a monster.", nameof(creatureCreationArguments));
                    }

                    return new Monster(monsterType, this.ItemFactory);
                case CreatureType.Player:

                    if (creatureCreationArguments == null ||
                        creatureCreationArguments.Metadata == null ||
                        !(creatureCreationArguments is PlayerCreationArguments playerCreationArguments))
                    {
                        throw new ArgumentException("Invalid creation arguments for a player.", nameof(creatureCreationArguments));
                    }

                    return new Player(
                        playerCreationArguments.Client,
                        playerCreationArguments.Metadata.Id,
                        playerCreationArguments.Metadata.Name,
                        playerCreationArguments.Metadata.MaxHitpoints,
                        playerCreationArguments.Metadata.MaxManapoints,
                        playerCreationArguments.Metadata.Corpse,
                        playerCreationArguments.Metadata.MaxHitpoints,      // TODO: current hitpoints.
                        playerCreationArguments.Metadata.MaxManapoints);    // TODO: current mana points.
            }

            throw new NotSupportedException($"{nameof(CreatureFactory)} does not support creation of creatures with type {creatureCreationArguments.Type}.");
        }
    }
}
