using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class ClearOnlinePlayersResultPacket : PacketOutgoing
    {
        public ushort ClearedCount { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(0x00);
            message.AddUInt16(ClearedCount);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}