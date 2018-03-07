using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
	public class AuthenticationPacket : PacketIncoming, IAuthenticationInfo
    {
        public byte Unknown { get; private set; }
        public string Password { get; private set; }
        public string WorldName { get; private set; }

        public AuthenticationPacket(NetworkMessage message)
            : base(message)
        {
        }
        
        public override void Parse(NetworkMessage message)
        {
            Unknown = message.GetByte();
            Password = message.GetString();
            WorldName = message.GetString();
        }
    }
}
