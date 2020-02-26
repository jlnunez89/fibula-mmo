// -----------------------------------------------------------------
// <copyright file="Notification.cs" company="2Dudes">
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
    using System.Linq;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Scheduling;
    using Serilog;

    /// <summary>
    /// Abstract class that represents a notification to a player's connection.
    /// Notifications are basically any message that the server sends to the client of a specific player.
    /// </summary>
    public abstract class Notification : BaseEvent, INotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger.</param>
        protected Notification(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Gets the function for determining target connections for this notification.
        /// </summary>
        protected abstract Func<IEnumerable<IConnection>> TargetConnectionsFunction { get; }

        /// <summary>
        /// Sends the notification using the supplied connection.
        /// </summary>
        public override void Execute()
        {
            try
            {
                IEnumerable<IConnection> connections = this.TargetConnectionsFunction?.Invoke();

                if (connections == null)
                {
                    this.Logger.Warning($"Attempted to send '{this.GetType().Name}' without any connections targets.");

                    return;
                }

                foreach (var connection in connections)
                {
                    INetworkMessage outboundMessage = new NetworkMessage();
                    IEnumerable<IOutgoingPacket> outgoingPackets = this.Prepare(connection.PlayerId);

                    if (outgoingPackets == null || !outgoingPackets.Any())
                    {
                        continue;
                    }

                    foreach (var packet in outgoingPackets)
                    {
                        packet.WriteToMessage(outboundMessage);
                    }

                    connection.Send(outboundMessage.Copy());
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error($"Error while sending {this.GetType().Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="playerId">The id of the player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s, the ones to be sent.</returns>
        protected abstract IEnumerable<IOutgoingPacket> Prepare(uint playerId);
    }
}
