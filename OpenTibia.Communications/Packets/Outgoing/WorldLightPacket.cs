using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class WorldLightPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.WorldLight;

        public byte Level { get; set; }

        public byte Color { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddByte(Level);
            message.AddByte(Color);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
