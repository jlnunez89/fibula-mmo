using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class AnimatedTextPacket : PacketOutgoing
    {
        public Location Location { get; set; }

        public TextColor Color { get; set; }

        public string Text { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.AnimatedText;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddLocation(Location);
            message.AddByte((byte)Color);
            message.AddString(Text);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
