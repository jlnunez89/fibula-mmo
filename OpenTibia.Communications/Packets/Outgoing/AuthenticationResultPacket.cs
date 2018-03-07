using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class AuthenticationResultPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public bool HadError { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte((byte) (HadError ? 0x01 : 0x00));
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
