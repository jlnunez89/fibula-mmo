using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public abstract class PacketOutgoing : IPacketOutgoing
    {
        public abstract byte PacketType { get; }

        public abstract void Add(NetworkMessage message);
        public abstract void CleanUp();
    }
}
