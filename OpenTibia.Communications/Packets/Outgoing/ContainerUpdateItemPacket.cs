using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class ContainerUpdateItemPacket : PacketOutgoing
    {
        public byte Index { get; set; }
        public byte ContainerId { get; set; }
        public IItem Item { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.ContainerUpdateItem;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddByte(ContainerId);
            message.AddByte(Index);
            message.AddItem(Item);
        }

        public override void CleanUp()
        {
            Item = null;
        }
    }
}