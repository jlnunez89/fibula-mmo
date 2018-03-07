using System;
using System.Collections.Generic;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Configuration;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Handlers.Management
{
    internal class AuthenticationHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandlePacket(NetworkMessage message, Connection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            var authPacket = new AuthenticationPacket(message);

            var result = authPacket.Password.Equals(ServiceConfiguration.GetConfiguration().QueryManagerPassword);
            
            connection.IsAuthenticated = result;

            ResponsePackets.Add(new AuthenticationResultPacket
            {
                HadError = !result
            });
        }
    }
}