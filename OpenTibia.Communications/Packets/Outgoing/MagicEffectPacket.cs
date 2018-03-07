using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class MagicEffectPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.MagicEffect;

        public Location Location { get; set; }

        public EffectT Effect { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddLocation(Location);
            message.AddByte((byte)Effect);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
