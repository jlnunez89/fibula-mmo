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
    internal class FinishAuctionsHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandlePacket(NetworkMessage message, Connection connection)
        {
            var finishAuctionsPacket = new DefaultReadPacket(message);

            using (var otContext = new OpenTibiaDbContext())
            {
                var housesJustAssigned = otContext.AssignedHouses.Where(h => h.Virgin > 0);

                foreach (var house in housesJustAssigned)
                {
                    house.Virgin = 0;
                }
                
                otContext.SaveChanges();

                ResponsePackets.Add(new FinishAuctionsResultPacket
                {
                    Houses = housesJustAssigned.ToList()
                });
            }
        }
    }
}