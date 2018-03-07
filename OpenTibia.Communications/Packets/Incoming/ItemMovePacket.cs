using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class ItemMovePacket : PacketIncoming
    {
        public Location FromLocation { get; private set; }
        public Location ToLocation { get; private set; }
        public byte FromStackPos { get; private set; }
        public ushort ClientId { get; private set; }
        public byte Count { get; private set; }

        public ItemMovePacket(NetworkMessage message)
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

            ClientId = message.GetUInt16();
            FromStackPos = message.GetByte();

            ToLocation = new Location
            {
                X = message.GetUInt16(),
                Y = message.GetUInt16(),
                Z = (sbyte)message.GetByte()
            };

            Count = message.GetByte();
        }
    }
}
