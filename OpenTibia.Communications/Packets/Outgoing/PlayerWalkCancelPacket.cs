using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class PlayerWalkCancelPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.PlayerWalkCancel;

        public Direction Direction { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddByte((byte)Direction);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
