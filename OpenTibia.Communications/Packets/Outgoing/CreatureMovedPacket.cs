using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class CreatureMovedPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.CreatureMoved;

        public Location FromLocation { get; set; }
        public byte FromStackpos { get; set; }
        public Location ToLocation { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddLocation(FromLocation);
            message.AddByte(FromStackpos);
            message.AddLocation(ToLocation);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
