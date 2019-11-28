// -----------------------------------------------------------------
// <copyright file="CreatureRemovedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Notifications
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents a notification for a creature being removed.
    /// </summary>
    internal class CreatureRemovedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureRemovedNotification"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="creatureFinder">A reference to the creature finder instance.</param>
        /// <param name="determineTargetConnectionsFunction">A function to determine the target connections of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureRemovedNotification(ILogger logger, ICreatureFinder creatureFinder, Func<IEnumerable<IConnection>> determineTargetConnectionsFunction, CreatureRemovedNotificationArguments arguments)
            : base(logger)
        {
            determineTargetConnectionsFunction.ThrowIfNull(nameof(determineTargetConnectionsFunction));
            arguments.ThrowIfNull(nameof(arguments));

            this.CreatureFinder = creatureFinder;
            this.TargetConnectionsFunction = determineTargetConnectionsFunction;
            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureRemovedNotificationArguments Arguments { get; }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the function for determining target connections for this notification.
        /// </summary>
        protected override Func<IEnumerable<IConnection>> TargetConnectionsFunction { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="playerId">The id of the player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s, the ones to be sent.</returns>
        protected override IEnumerable<IOutgoingPacket> Prepare(uint playerId)
        {
            var player = this.CreatureFinder.FindCreatureById(playerId);

            if (player == null || !player.CanSee(this.Arguments.Creature) || !player.CanSee(this.Arguments.Creature.Location))
            {
                return null;
            }

            var packets = new List<IOutgoingPacket>
            {
                new RemoveAtStackposPacket(this.Arguments.Creature.Location, this.Arguments.OldStackPosition),
            };

            if (this.Arguments.RemoveEffect != AnimatedEffect.None)
            {
                packets.Add(new MagicEffectPacket(this.Arguments.Creature.Location, AnimatedEffect.Puff));
            }

            return packets;
        }
    }
}