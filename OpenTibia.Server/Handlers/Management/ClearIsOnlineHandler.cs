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
    internal class ClearIsOnlineHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandlePacket(NetworkMessage message, Connection connection)
        {
            var clearOnlinePacket = new DefaultReadPacket(message);

            using (var otContext = new OpenTibiaDbContext())
            {
                var onlinePlayers = otContext.Players.Where(p => p.Online > 0).ToList();

                foreach (var player in onlinePlayers)
                {
                    player.Online = 0;
                }
                
                otContext.SaveChanges();

                ResponsePackets.Add(new ClearOnlinePlayersResultPacket
                {
                    ClearedCount = (ushort)onlinePlayers.Count
                });
            }
        }
    }
}