using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class PlayerLoginRejectionPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public byte Reason { get; set; }
        
        public override void Add(NetworkMessage message)
        {
            message.AddByte(0x01); // Should always be 1 for this packet; means there was an error.
            message.AddByte(Reason);
            message.AddByte(0xFF); // EOM ?
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
