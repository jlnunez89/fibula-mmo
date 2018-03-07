using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class CharacterDeathPacket : PacketIncoming, ICharacterDeathInfo
    {
        public int VictimId { get; set; }
        public short VictimLevel { get; set; }
        public int KillerId { get; set; }
        public string KillerName { get; set; }
        public byte Unjustified { get; set; }
        public long Timestamp { get; set; }

        public CharacterDeathPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            VictimId = (int)message.GetUInt32();
            VictimLevel = (short)message.GetUInt16();
            KillerId = (int)message.GetUInt32();
            KillerName = message.GetString();
            Unjustified = message.GetByte();
            Timestamp = message.GetUInt32();
        }
    }
}
