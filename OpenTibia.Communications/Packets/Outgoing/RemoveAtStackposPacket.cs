using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class RemoveAtStackposPacket : PacketOutgoing
    {
        public Location Location { get; set; }
        public byte Stackpos { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.RemoveAtStackpos;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddLocation(Location);
            message.AddByte(Stackpos);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
