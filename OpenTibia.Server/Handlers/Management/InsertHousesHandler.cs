using System.Collections.Generic;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Handlers.Management
{
    internal class InsertHousesHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandlePacket(NetworkMessage message, Connection connection)
        {
            var inserHousePacket = new InsertHousePacket(message);

            // TODO: actually update house info?
            //using (OpenTibiaDbContext otContext = new OpenTibiaDbContext())
            //{
            ResponsePackets.Add(new DefaultNoErrorPacket());
            //}

            //return new DefaultErrorPacket();
        }
    }
}