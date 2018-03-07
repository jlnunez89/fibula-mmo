using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class FinishAuctionsPacket : PacketIncoming, IFinishAuctionsInfo
    {
        public uint VictimId { get; set; }
        public ushort VictimLevel { get; set; }
        public uint KillerId { get; set; }
        public string KillerName { get; set; }
        public byte Unjustified { get; set; }
        public uint Timestamp { get; set; }

        public FinishAuctionsPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            VictimId = message.GetUInt32();
            VictimLevel = message.GetUInt16();
            KillerId = message.GetUInt32();
            KillerName = message.GetString();
            Unjustified = message.GetByte();
            Timestamp = message.GetUInt32();
        }
    }
}
