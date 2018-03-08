// <copyright file="Notification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public abstract class Notification : INotification
    {
        public Connection Connection { get; }

        public IList<IPacketOutgoing> ResponsePackets { get; protected set; }

        protected Notification(Connection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            this.Connection = connection;
            this.ResponsePackets = new List<IPacketOutgoing>();
        }

        protected Notification(Connection connection, params IPacketOutgoing[] packets)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            this.Connection = connection;

            foreach (var packet in packets)
            {
                this.ResponsePackets.Add(packet);
            }
        }

        public abstract void Prepare();

        public void Send()
        {
            if (!this.ResponsePackets.Any())
            {
                return;
            }

            var networkMessage = new NetworkMessage(4);

            foreach (var packet in this.ResponsePackets)
            {
                networkMessage.AddPacket(packet);
            }

            this.Connection.Send(networkMessage);

            // foreach (var packet in ResponsePackets)
            // {
            //    packet.CleanUp();
            // }
        }
    }
}
