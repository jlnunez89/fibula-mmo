using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class ContainerAddItemPacket : PacketOutgoing
    {
        public byte ContainerId { get; set; }
        public IItem Item { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.ContainerAddItem;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            
            message.AddByte(ContainerId);
            message.AddItem(Item);
        }

        public override void CleanUp()
        {
            Item = null;
        }
    }
}