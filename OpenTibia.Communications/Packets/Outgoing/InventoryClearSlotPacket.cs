using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class InventoryClearSlotPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.InventoryEmpty;

        public Slot Slot { get; set; }
        
        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddByte((byte)Slot);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
