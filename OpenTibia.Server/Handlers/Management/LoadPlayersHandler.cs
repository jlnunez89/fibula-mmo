using System;
using System.Collections.Generic;
using System.Linq;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Data;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Handlers.Management
{
    internal class LoadPlayersHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandlePacket(NetworkMessage message, Connection connection)
        {
            var loadPlayersPacket = new DefaultReadPacket(message);
            
            using (var otContext = new OpenTibiaDbContext())
            {
                var thirtyDaysBack = DateTime.Today.AddDays(-30).ToFileTimeUtc();

                var loadedPlayers = otContext.Players.Where(p => p.Lastlogin > thirtyDaysBack);

                ResponsePackets.Add(new LoadPlayersResultPacket
                {
                    LoadedPlayers = loadedPlayers.ToList()
                });
            }
        }
    }
}