using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class ProjectilePacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.CreatureMoved;

        public Location FromLocation { get; set; }
        public Location ToLocation { get; set; }
        public ShootTypeT ShootType { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddLocation(FromLocation);
            message.AddLocation(ToLocation);
            message.AddByte((byte)ShootType);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
