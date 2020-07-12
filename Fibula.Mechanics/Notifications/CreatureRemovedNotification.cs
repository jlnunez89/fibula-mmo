// -----------------------------------------------------------------
// <copyright file="CreatureRemovedNotification.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Notifications
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a notification for a creature being removed.
    /// </summary>
    public class CreatureRemovedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureRemovedNotification"/> class.
        /// </summary>
        /// <param name="findTargetPlayers">A function to determine the target players of this notification.</param>
        /// <param name="creature">The creature being removed.</param>
        /// <param name="oldStackPos">The position in the stack of the creature being removed.</param>
        /// <param name="removeEffect">Optional. An effect to add when removing the creature.</param>
        public CreatureRemovedNotification(Func<IEnumerable<IPlayer>> findTargetPlayers, ICreature creature, byte oldStackPos, AnimatedEffect removeEffect = AnimatedEffect.None)
            : base(findTargetPlayers)
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

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="context">The context of this notification.</param>
        /// <param name="player">The player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s, the ones to be sent.</returns>
        protected override IEnumerable<IOutboundPacket> Prepare(INotificationContext context, IPlayer player)
        {
            if (player == null || !player.CanSee(this.Creature) || !player.CanSee(this.Creature.Location))
            {
                return null;
            }

            var packets = new List<IOutboundPacket>
            {
                new RemoveAtLocationPacket(this.Creature.Location, this.StackPosition),
            };

            if (this.RemoveEffect != AnimatedEffect.None)
            {
                packets.Add(new MagicEffectPacket(this.Creature.Location, this.RemoveEffect));
            }

            return packets;
        }
    }
}
