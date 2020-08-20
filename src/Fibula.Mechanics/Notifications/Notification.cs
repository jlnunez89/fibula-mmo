// -----------------------------------------------------------------
// <copyright file="Notification.cs" company="2Dudes">
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
    using Fibula.Common.Utilities;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Delegates;
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
        /// Event to call when the notification is sent.
        /// </summary>
        public event OnSent Sent;

        /// <summary>
        /// Gets a string representing this notification's event type.
        /// </summary>
        public override string EventType => this.GetType().Name;

        /// <summary>
        /// Gets or sets a value indicating whether the event can be cancelled.
        /// </summary>
        public override bool CanBeCancelled { get; protected set; }

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

            this.Send(notificationContext);
        }

        /// <summary>
        /// Sends the notification to the players intented.
        /// </summary>
        /// <param name="context">The context for this notification.</param>
        public void Send(INotificationContext context)
        {
            try
            {
                IEnumerable<IPlayer> targetPlayers = this.FindTargetPlayers?.Invoke();

                if (targetPlayers == null || !targetPlayers.Any())
                {
                    // This one is verbose because it is really common, for example, expiring items or spawning creatures,
                    // which mostly tend to happen while there are no players around.
                    context.Logger?.Verbose($"Found no targets for {this.GetType().Name}, skipping.");
                    return;
                }

                foreach (var player in targetPlayers)
                {
                    if (player == null)
                    {
                        continue;
                    }

                    INetworkMessage outboundMessage = new NetworkMessage();
                    IEnumerable<IOutboundPacket> outgoingPackets = this.Prepare(context, player);

                    if (outgoingPackets == null || !outgoingPackets.Any())
                    {
                        continue;
                    }

                    player.Client?.Send(outgoingPackets);

                    this.Sent?.Invoke(player.Client);
                }
            }
            catch (Exception ex)
            {
                context.Logger?.Error($"Error while sending {this.GetType().Name}: {ex.Message}");
            }

            this.Sent = null;
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
