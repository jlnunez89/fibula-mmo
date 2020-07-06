// -----------------------------------------------------------------
// <copyright file="CreatureRemovedNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Notifications.Arguments
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Notifications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents arguments for the creature being removed notification.
    /// </summary>
    public class CreatureRemovedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureRemovedNotificationArguments"/> class.
        /// </summary>
        /// <param name="creature">The creature being removed.</param>
        /// <param name="oldStackPos">The position in the stack of the creature being removed.</param>
        /// <param name="removeEffect">Optional. An effect to add when removing the creature.</param>
        public CreatureRemovedNotificationArguments(ICreature creature, byte oldStackPos, AnimatedEffect removeEffect = AnimatedEffect.None)
        {
            creature.ThrowIfNull(nameof(creature));

            this.Creature = creature;
            this.StackPosition = oldStackPos;
            this.RemoveEffect = removeEffect;
        }

        /// <summary>
        /// Gets the effect to send when removing the creature.
        /// </summary>
        public AnimatedEffect RemoveEffect { get; }

        /// <summary>
        /// Gets the stack position of the creature being removed.
        /// </summary>
        public byte StackPosition { get; }

        /// <summary>
        /// Gets the creature being removed.
        /// </summary>
        public ICreature Creature { get; }
    }
}