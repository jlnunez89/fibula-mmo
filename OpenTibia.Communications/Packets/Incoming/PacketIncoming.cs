using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Incoming
{
    public abstract class PacketIncoming : IPacketIncoming
    {
        public PacketIncoming(NetworkMessage message)
        {
            NetworkMessage = message;
            Parse(NetworkMessage);
        }

        public NetworkMessage NetworkMessage { get; }

        public abstract void Parse(NetworkMessage message);
    }
}
