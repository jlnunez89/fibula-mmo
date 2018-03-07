using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class CreatePlayerListResultPacket : PacketOutgoing
    {
        public bool IsNewRecord { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(0x00); // No error flag
            message.AddByte((byte) (IsNewRecord ? 0xFF : 0x00));
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}