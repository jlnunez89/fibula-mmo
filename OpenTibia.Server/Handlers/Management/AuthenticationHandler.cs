// <copyright file="AuthenticationHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers.Management
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Configuration;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    internal class AuthenticationHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            var authPacket = new AuthenticationPacket(message);

            var result = authPacket.Password.Equals(ServiceConfiguration.GetConfiguration().QueryManagerPassword);

            connection.IsAuthenticated = result;

            this.ResponsePackets.Add(new AuthenticationResultPacket
            {
                HadError = !result
            });
        }
    }
}