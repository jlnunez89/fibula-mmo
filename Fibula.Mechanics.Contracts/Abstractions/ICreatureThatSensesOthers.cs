// -----------------------------------------------------------------
// <copyright file="ICreatureThatSensesOthers.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Delegates;

    /// <summary>
    /// Interface for any <see cref="ICreature"/> in the game that can sense others.
    /// </summary>
    public interface ICreatureThatSensesOthers : ICreature
    {
        /// <summary>
        /// Event called when this creature senses another.
        /// </summary>
        event OnCreatureSensed CreatureSensed;

        /// <summary>
        /// Event called when this creature sees another.
        /// </summary>
        event OnCreatureSeen CreatureSeen;

        /// <summary>
        /// Event called when this creature loses track of a sensed creature.
        /// </summary>
        event OnCreatureLost CreatureLost;

        /// <summary>
        /// Gets the creatures who are sensed by this creature.
        /// </summary>
        IEnumerable<ICreature> TrackedCreatures { get; }

        /// <summary>
        /// Flags a creature as being sensed.
        /// </summary>
        /// <param name="creature">The creature which is sensed.</param>
        void StartSensingCreature(ICreature creature);

        /// <summary>
        /// Flags a creature as no longed being sensed.
        /// </summary>
        /// <param name="creature">The creature that is lost.</param>
        void StopSensingCreature(ICreature creature);
    }
}
