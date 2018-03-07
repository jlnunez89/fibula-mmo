using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class PlayerLoginPacket : PacketIncoming, IPlayerLoginInfo
    {
        public ushort Os { get; set; }
        public ushort Version { get; set; }
        public uint[] XteaKey { get; set; }
        public bool IsGm { get; set; }
        public uint AccountNumber { get; set; }
        public string CharacterName { get; set; }
        public string Password { get; set; }

        public PlayerLoginPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            XteaKey = new uint[4];
            XteaKey[0] = message.GetUInt32();
            XteaKey[1] = message.GetUInt32();
            XteaKey[2] = message.GetUInt32();
            XteaKey[3] = message.GetUInt32();

            Os = message.GetUInt16();
            Version = message.GetUInt16();

            IsGm = message.GetByte() > 0;

            AccountNumber = message.GetUInt32();
            CharacterName = message.GetString();
            Password = message.GetString();
        }
    }
}
