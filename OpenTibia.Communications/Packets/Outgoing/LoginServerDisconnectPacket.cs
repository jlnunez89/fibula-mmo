using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class LoginServerDisconnectPacket : PacketOutgoing
    {
        public string Reason { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.Disconnect;

        public LoginServerDisconnectPacket()
        {
            Reason = string.Empty;
        }

        public LoginServerDisconnectPacket(string reason)
        {
            Reason = reason;
        }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddString(Reason);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
