using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class ContainerRemoveItemPacket : PacketOutgoing
    {
        public byte Index { get; set; }
        public byte ContainerId { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.ContainerRemoveItem;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddByte(ContainerId);
            message.AddByte(Index);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}