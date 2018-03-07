using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class AttackPacket : PacketIncoming
    {
        public uint CreatureId { get; private set; }

        public AttackPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            CreatureId = message.GetUInt32();
        }
    }
}
