// -----------------------------------------------------------------
// <copyright file="CreatureFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures
{
    using System;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Items.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a factory of creatures.
    /// </summary>
    public class CreatureFactory : ICreatureFactory
    {
        ///// <summary>
        ///// Stores the catalog of monster types, which is a mapping of the raceId and the type.
        ///// </summary>
        //private readonly IDictionary<ushort, IMonsterType> monsterTypeCatalog;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="CreatureFactory"/> class.
        ///// </summary>
        ///// <param name="monsterLoader">A reference to the monster type loader in use.</param>
        ///// <param name="itemFactory">A reference to the item factory in use.</param>
        //public CreatureFactory(IMonsterTypeLoader monsterLoader, IItemFactory itemFactory)
        //{
        //    monsterLoader.ThrowIfNull(nameof(monsterLoader));
        //    itemFactory.ThrowIfNull(nameof(itemFactory));

        //    this.monsterTypeCatalog = monsterLoader.LoadTypes();

        //    this.ItemFactory = itemFactory;
        //}

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

                    //case CreatureType.Monster:
                    //    // Find the actual monster type to init with.
                    //    var raceId = Convert.ToUInt16(creatureMetadata.Identifier);

                    //    if (this.monsterTypeCatalog.TryGetValue(raceId, out IMonsterType monsterType))
                    //    {
                    //        return new Monster(monsterType, this.ItemFactory);
                    //    }

                    //    throw new InvalidOperationException($"{nameof(creatureMetadata)} has an invalid race Id {creatureMetadata.Identifier}. No monster could be created.");
            }

            throw new NotSupportedException($"{nameof(CreatureFactory)} does not support creation of creatures with type {creatureCreationArguments.Type}.");
        }
    }
}
