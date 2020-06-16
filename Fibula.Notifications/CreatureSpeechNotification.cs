// -----------------------------------------------------------------
// <copyright file="CreatureSpeechNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Notifications
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Server.Creatures.Contracts.Abstractions;
    using Fibula.Server.Notifications.Arguments;
    using Fibula.Server.Notifications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a notification for a creature speech.
    /// </summary>
    public class CreatureSpeechNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureSpeechNotification"/> class.
        /// </summary>
        /// <param name="findTargetPlayers">A function to determine the target players of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureSpeechNotification(Func<IEnumerable<IPlayer>> findTargetPlayers, CreatureSpokeNotificationArguments arguments)
            : base(findTargetPlayers)
        {
            arguments.ThrowIfNull(nameof(arguments));

            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureSpokeNotificationArguments Arguments { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="context">The context of this notification.</param>
        /// <param name="player">The player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s, the ones to be sent.</returns>
        protected override IEnumerable<IOutboundPacket> Prepare(INotificationContext context, IPlayer player)
        {
            return new CreatureSpeechPacket(
                this.Arguments.Creature.Id,
                this.Arguments.Creature.Name,
                this.Arguments.SpeechType,
                this.Arguments.Text,
                this.Arguments.Creature.Location,
                this.Arguments.ChannelId,
                (uint)DateTimeOffset.Now.ToUnixTimeMilliseconds()).YieldSingleItem();
        }
    }
}