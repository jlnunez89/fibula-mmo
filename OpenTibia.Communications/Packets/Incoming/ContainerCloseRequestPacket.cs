using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class ContainerCloseRequestPacket : PacketIncoming
    {
        public byte ContainerId { get; private set; }

        public ContainerCloseRequestPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            ContainerId = message.GetByte();
        }
    }
}
