using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class AddCreaturePacket : PacketOutgoing
    {
        public Location Location { get; set; }

        public ICreature Creature { get; set; }

        public bool AsKnown { get; set; }

        public uint RemoveThisCreatureId { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.AddAtStackpos;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddLocation(Location);
            message.AddCreature(Creature, AsKnown, RemoveThisCreatureId);
        }

        public override void CleanUp()
        {
            Creature = null;
        }
    }
}
