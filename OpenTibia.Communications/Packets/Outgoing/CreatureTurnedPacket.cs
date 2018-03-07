using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class CreatureTurnedPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.TransformThing;

        public ICreature Creature { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddLocation(Creature.Location);
            message.AddByte(Creature.GetStackPosition());
            message.AddUInt16(Creature.ThingId);
            message.AddUInt32(Creature.CreatureId);
            message.AddByte((byte)Creature.Direction);
        }

        public override void CleanUp()
        {
            Creature = null;
        }
    }
}
