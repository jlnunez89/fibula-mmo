using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class DefaultReadPacket : PacketIncoming
    {
        public byte[] InfoBytes { get; set; }
        
        public DefaultReadPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            var dataLength = message.Length - message.Position;
            InfoBytes = message.GetBytes(dataLength);
        }
    }
}
