using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class UpdateTilePacket : PacketOutgoing
    {
        public Location Location { get; set; }

        public byte[] DescriptionBytes { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.TileUpdate;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddLocation(Location);

            if (DescriptionBytes.Length > 0)
            {
                message.AddBytes(DescriptionBytes);
                message.AddByte(0x00); // skip count
            }
            else
            {
                message.AddByte(0x01); // skip count
            }

            message.AddByte(0xFF);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
