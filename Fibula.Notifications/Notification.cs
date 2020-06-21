// -----------------------------------------------------------------
// <copyright file="Notification.cs" company="2Dudes">
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
    using System.Linq;
    using Fibula.Common.Utilities;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Notifications.Contracts.Abstractions;
    using Fibula.Scheduling;
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Abstract class that represents a notification to a player's connection.
    /// Notifications are basically any message that the server sends to the client of a specific player.
    /// </summary>
    public abstract class Notification : BaseEvent, INotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        /// <param name="findTargetPlayers">A function to determine the target players of this notification.</param>
        protected Notification(Func<IEnumerable<IPlayer>> findTargetPlayers)
        {
            this.FindTargetPlayers = findTargetPlayers;
        }

        /// <summary>
        /// Gets the function for determining target players for this notification.
        /// </summary>
        private Func<IEnumerable<IPlayer>> FindTargetPlayers { get; }

        /// <summary>
        /// Sends the notification to the players intented.
        /// </summary>
        /// <param name="context">The context for this notification.</param>
        public override void Execute(IEventContext context)
        {
            context.ThrowIfNull(nameof(context));

            if (!typeof(INotificationContext).IsAssignableFrom(context.GetType()) || !(context is INotificationContext notificationContext))
            {
                throw new ArgumentException($"{nameof(context)} must be an {nameof(INotificationContext)}.");
            }

            try
            {
                IEnumerable<IPlayer> targetPlayers = this.FindTargetPlayers.Invoke();

                if (targetPlayers == null || !targetPlayers.Any())
                {
                    return;
                }

                foreach (var player in targetPlayers)
                {
                    if (player == null)
                    {
                        continue;
                    }

                    INetworkMessage outboundMessage = new NetworkMessage();
                    IEnumerable<IOutboundPacket> outgoingPackets = this.Prepare(notificationContext, player);

                    if (outgoingPackets == null || !outgoingPackets.Any())
                    {
                        continue;
                    }

                    player.Client?.Send(outgoingPackets);
                }
            }
            catch (Exception ex)
            {
                context.Logger?.Error($"Error while sending {this.GetType().Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends the notification to the players intented.
        /// </summary>
        /// <param name="context">The context for this notification.</param>
        public void Send(INotificationContext context)
        {
            this.Execute(context);
        }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="context">The context of this notification.</param>
        /// <param name="player">The player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s, the ones to be sent.</returns>
        protected abstract IEnumerable<IOutboundPacket> Prepare(INotificationContext context, IPlayer player);
    }
}
