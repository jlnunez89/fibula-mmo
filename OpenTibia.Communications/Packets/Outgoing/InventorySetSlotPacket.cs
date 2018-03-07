using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class InventorySetSlotPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.InventoryItem;

        public Slot Slot { get; set; }

        public IItem Item { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddByte((byte)Slot);
            message.AddItem(Item);
        }

        public override void CleanUp()
        {
            Item = null;
        }
    }
}
