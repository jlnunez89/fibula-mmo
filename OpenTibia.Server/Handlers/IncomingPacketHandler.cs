using System.Collections.Generic;
using OpenTibia.Communications;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Handlers
{
    public abstract class IncomingPacketHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; protected set; }

        protected IncomingPacketHandler()
        {
            ResponsePackets = new List<IPacketOutgoing>();
        }

        public abstract void HandlePacket(NetworkMessage message, Connection connection);
    }
}