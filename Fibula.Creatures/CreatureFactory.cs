﻿// -----------------------------------------------------------------
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
                // case CreatureType.Monster:
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
