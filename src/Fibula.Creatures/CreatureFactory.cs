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
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Data.Entities.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a factory of creatures.
    /// </summary>
    public class CreatureFactory : ICreatureFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureFactory"/> class.
        /// </summary>
        /// <param name="applicationContext">A reference to the application context.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        public CreatureFactory(IApplicationContext applicationContext, IItemFactory itemFactory)
        {
            applicationContext.ThrowIfNull(nameof(applicationContext));
            itemFactory.ThrowIfNull(nameof(itemFactory));

            this.ApplicationContext = applicationContext;
            this.ItemFactory = itemFactory;
        }

        /// <summary>
        /// Gets the application context.
        /// </summary>
        public IApplicationContext ApplicationContext { get; }

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
                    if (creatureCreationArguments.Metadata?.Id == null)
                    {
                        throw new ArgumentException("Invalid metadata in creation arguments for a monster.", nameof(creatureCreationArguments));
                    }

                    using (var unitOfWork = this.ApplicationContext.CreateNewUnitOfWork())
                    {
                        if (!(unitOfWork.MonsterTypes.GetById(creatureCreationArguments.Metadata.Id) is IMonsterTypeEntity monsterType))
                        {
                            throw new ArgumentException($"Unknown monster with Id {creatureCreationArguments.Metadata.Id} in creation arguments for a monster.", nameof(creatureCreationArguments));
                        }

                        // TODO: go through inventory composition here.
                        // For each inventory item (and chance or whatever), add the items to the monster (as IContainerThing),
                        // which will make the items fall in place, and also not have us have to pass the item factory into the monster instance, because it's weird.
                        return new Monster(monsterType, this.ItemFactory);
                    }

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
                        playerCreationArguments.Metadata.Corpse);
            }

            throw new NotSupportedException($"{nameof(CreatureFactory)} does not support creation of creatures with type {creatureCreationArguments.Type}.");
        }
    }
}
