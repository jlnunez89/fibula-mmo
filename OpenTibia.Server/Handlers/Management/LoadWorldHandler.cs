using System.Collections.Generic;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Configuration;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Handlers.Management
{
    internal class LoadWorldHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandlePacket(NetworkMessage message, Connection connection)
        {
            // No incoming packet is required to load here.
            
            var gameConfig = ServiceConfiguration.GetConfiguration();

            ResponsePackets.Add(new WorldConfigPacket
            {
                WorldType = (byte)gameConfig.WorldType,
                DailyResetHour = gameConfig.DailyResetHour,
                IpAddressBytes = gameConfig.PrivateGameIpAddress.GetAddressBytes(),
                Port = gameConfig.PrivateGamePort,
                MaximumTotalPlayers = gameConfig.MaximumTotalPlayers,
                PremiumMainlandBuffer = gameConfig.PremiumMainlandBuffer,
                MaximumRookgardians = gameConfig.MaximumRookgardians,
                PremiumRookgardiansBuffer = gameConfig.PremiumRookgardiansBuffer
            });
        }
    }
}