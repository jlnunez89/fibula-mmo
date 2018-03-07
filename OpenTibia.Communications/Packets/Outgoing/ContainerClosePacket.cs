using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class ContainerClosePacket : PacketOutgoing
    {
        public byte ContainerId { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.ContainerClose;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            
            message.AddByte(ContainerId);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}