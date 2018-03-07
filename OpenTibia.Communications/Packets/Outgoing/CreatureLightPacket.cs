using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class CreatureLightPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.CreatureLight;

        public ICreature Creature { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddUInt32(Creature.CreatureId);
            message.AddByte(Creature.LightBrightness); // light level
            message.AddByte(Creature.LightColor); // color
        }

        public override void CleanUp()
        {
            Creature = null;
        }
    }
}
