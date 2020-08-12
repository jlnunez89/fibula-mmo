// -----------------------------------------------------------------
// <copyright file="GenericNotification.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a generic notification.
    /// </summary>
    public class GenericNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericNotification"/> class.
        /// </summary>
        /// <param name="findTargetPlayers">A function to determine the target players of this notification.</param>
        /// <param name="outgoingPackets">The packets to send as part of this notification.</param>
        public GenericNotification(Func<IEnumerable<IPlayer>> findTargetPlayers, params IOutboundPacket[] outgoingPackets)
            : base(findTargetPlayers)
        {
            if (outgoingPackets == null || !outgoingPackets.Any())
            {
                throw new ArgumentNullException(nameof(outgoingPackets));
            }

            this.OutgoingPackets = outgoingPackets;
        }

        /// <summary>
        /// Gets the packets to be included in this notification.
        /// </summary>
        public IEnumerable<IOutboundPacket> OutgoingPackets { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="context">The context of this notification.</param>
        /// <param name="player">The player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s, the ones to be sent.</returns>
        protected override IEnumerable<IOutboundPacket> Prepare(INotificationContext context, IPlayer player)
        {
            return this.OutgoingPackets;
        }
    }
}
