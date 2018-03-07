using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class ContainerUpPacket : PacketIncoming
    {
        public byte ContainerId { get; private set; }

        public ContainerUpPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            ContainerId = message.GetByte();
        }
    }
}
