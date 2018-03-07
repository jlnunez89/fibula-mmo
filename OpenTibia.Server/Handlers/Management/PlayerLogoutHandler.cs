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
    internal class PlayerLogoutHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandlePacket(NetworkMessage message, Connection connection)
        {
            var playerLogoutPacket = new ManagementPlayerLogoutPacket(message);

            using (var otContext = new OpenTibiaDbContext())
            {
                var playerRecord = otContext.Players.Where(p => p.Account_Id == playerLogoutPacket.AccountId).FirstOrDefault();

                if (playerRecord != null)
                {
                    playerRecord.Level = playerLogoutPacket.Level;
                    playerRecord.Vocation = playerLogoutPacket.Vocation;
                    playerRecord.Residence = playerLogoutPacket.Residence;
                    playerRecord.Lastlogin = playerLogoutPacket.LastLogin;

                    playerRecord.Online = 0;

                    var onlineRecord = otContext.Online.Where(o => o.Name.Equals(playerRecord.Charname)).FirstOrDefault();

                    if(onlineRecord != null)
                    {
                        otContext.Online.Remove(onlineRecord);
                    }
                    
                    otContext.SaveChanges();

                    ResponsePackets.Add(new DefaultNoErrorPacket());
                }
            }
        }
    }
}