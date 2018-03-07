using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class RuleViolationPacket : PacketIncoming, IRuleViolationInfo
    {
        public int GamemasterId { get; set; }
        public string CharacterName { get; set; }
        public string IpAddress { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }

        public RuleViolationPacket(NetworkMessage message) : base(message)
        {

        }

        public override void Parse(NetworkMessage message)
        {
            GamemasterId = (int)message.GetUInt32();
            CharacterName = message.GetString();
            IpAddress = message.GetString();
            Reason = message.GetString();
            Comment = message.GetString();
        }
    }
}
