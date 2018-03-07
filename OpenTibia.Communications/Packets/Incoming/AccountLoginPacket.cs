using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class AccountLoginPacket : PacketIncoming, IAccountLoginInfo
    {
        public uint AccountNumber { get; set; }
        public string Password { get; set; }
        public uint[] XteaKey { get; set; }
        //public byte GmMode { get; set; }
        //public string CharacterName { get; set; }

        public AccountLoginPacket(NetworkMessage message) : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            //this.GmMode = message.GetByte();

            XteaKey = new uint[4];
            XteaKey[0] = message.GetUInt32();
            XteaKey[1] = message.GetUInt32();
            XteaKey[2] = message.GetUInt32();
            XteaKey[3] = message.GetUInt32();

            AccountNumber = message.GetUInt32();
            //this.CharacterName = message.GetString();
            Password = message.GetString();
        }
    }
}
