using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Utilities;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class ServerStatusPacket : PacketOutgoing
    {
        public string Data { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public override void Add(NetworkMessage message)
        {
            message.AddBytes(Data.ToByteArray());
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
