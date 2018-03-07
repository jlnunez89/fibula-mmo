using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class ManagementPlayerLoginPacket : PacketIncoming, IManagementPlayerLoginInfo
    {
        public uint AccountNumber { get; set; }
        public string CharacterName { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
        
        public ManagementPlayerLoginPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            AccountNumber = message.GetUInt32();
            CharacterName = message.GetString();
            Password = message.GetString();
            IpAddress = message.GetString();
        }
    }
}
