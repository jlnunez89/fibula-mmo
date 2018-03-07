using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
	public class NewConnectionPacket : PacketIncoming, INewConnectionInfo
    {
        public ushort Os { get; set; }
        public ushort Version { get; set; }

        public NewConnectionPacket(NetworkMessage message)
            : base(message)
        {
        }
        
        public override void Parse(NetworkMessage message)
        {
            Os = message.GetUInt16();
            Version = message.GetUInt16();

            message.SkipBytes(12);
        }
	}
}
