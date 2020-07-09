// -----------------------------------------------------------------
// <copyright file="CreatureTurnedNotificationArguments.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Notifications.Arguments
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Notifications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents arguments for the creature turned notification.
    /// </summary>
    public class CreatureTurnedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureTurnedNotificationArguments"/> class.
        /// </summary>
        /// <param name="creature">The creature that turned.</param>
        /// <param name="creatureStackPosition">The position in the stack of the creature that turned.</param>
        /// <param name="turnEffect">Optional. An effect of the turn.</param>
        public CreatureTurnedNotificationArguments(ICreature creature, byte creatureStackPosition, AnimatedEffect turnEffect = AnimatedEffect.None)
        {
            creature.ThrowIfNull(nameof(creature));

            this.Creature = creature;
            this.StackPosition = creatureStackPosition;
            this.TurnedEffect = turnEffect;
        }

        /// <summary>
        /// Gets the creature that turned.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Gets the position in the stack of the creatue.
        /// </summary>
        public byte StackPosition { get; }

        /// <summary>
        /// Gets the effect of the turn, if any.
        /// </summary>
        public AnimatedEffect TurnedEffect { get; }
    }
}
