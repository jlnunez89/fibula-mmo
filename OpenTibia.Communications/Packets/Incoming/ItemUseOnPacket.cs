using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class ItemUseOnPacket : PacketIncoming
    {
        public Location FromLocation { get; set; }
        public ushort FromSpriteId { get; set; }
        public byte FromStackPosition { get; set; }
        public Location ToLocation { get; set; }
        public ushort ToSpriteId { get; set; }
        public byte ToStackPosition { get; set; }

        public ItemUseOnPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            FromLocation = new Location
            {
                X = message.GetUInt16(),
                Y = message.GetUInt16(),
                Z = (sbyte)message.GetByte()
            };

            FromSpriteId = message.GetUInt16();

            FromStackPosition = message.GetByte();

            ToLocation = new Location
            {
                X = message.GetUInt16(),
                Y = message.GetUInt16(),
                Z = (sbyte)message.GetByte()
            };

            ToSpriteId = message.GetUInt16();

            ToStackPosition = message.GetByte();
        }
    }
}
