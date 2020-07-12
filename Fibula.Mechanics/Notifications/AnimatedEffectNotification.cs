// -----------------------------------------------------------------
// <copyright file="AnimatedEffectNotification.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a notification for animated effects.
    /// </summary>
    public class AnimatedEffectNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedEffectNotification"/> class.
        /// </summary>
        /// <param name="findTargetPlayers">A function to determine the target players of this notification.</param>
        /// <param name="location">The location of the animated text.</param>
        /// <param name="effect">The effect.</param>
        public AnimatedEffectNotification(
            Func<IEnumerable<IPlayer>> findTargetPlayers,
            Location location,
            AnimatedEffect effect = AnimatedEffect.None)
            : base(findTargetPlayers)
        {
            if (effect == AnimatedEffect.None)
            {
                throw new ArgumentException($"Invalid value for effect parameter: {effect}.", nameof(effect));
            }

            this.Location = location;
            this.Effect = effect;
        }

        /// <summary>
        /// Gets the location of the animated text.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the actual effect.
        /// </summary>
        public AnimatedEffect Effect { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="context">The context of this notification.</param>
        /// <param name="player">The player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s, the ones to be sent.</returns>
        protected override IEnumerable<IOutboundPacket> Prepare(INotificationContext context, IPlayer player)
        {
            return new MagicEffectPacket(this.Location, this.Effect).YieldSingleItem();
        }
    }
}
