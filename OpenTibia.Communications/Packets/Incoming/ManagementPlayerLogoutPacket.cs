using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class ManagementPlayerLogoutPacket : PacketIncoming, IPlayerLogoutInfo
    {
        public int AccountId { get; set; }
        public short Level { get; set; }
        public string Vocation { get; set; }
        public string Residence { get; set; }
        public int LastLogin { get; set; }

        public ManagementPlayerLogoutPacket(NetworkMessage message)
            : base(message)
        {

        }

        public override void Parse(NetworkMessage message)
        {
            AccountId = (int)message.GetUInt32();
            Level = (short)message.GetUInt16();
            Vocation = message.GetString();
            Residence = message.GetString();
            LastLogin = (int)message.GetUInt32();
        }
    }
}
