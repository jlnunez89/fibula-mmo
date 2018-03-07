using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class AddItemPacket : PacketOutgoing
    {
        public Location Location { get; set; }

        public IItem Item { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.AddAtStackpos;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddLocation(Location);
            message.AddItem(Item);
        }

        public override void CleanUp()
        {
            Item = null;
        }
    }
}
