using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class ItemUsePacket : PacketIncoming
    {
        public Location FromLocation { get; private set; }
        public byte FromStackPos { get; private set; }
        public ushort ClientId { get; private set; }
        public byte Index { get; private set; }

        public ItemUsePacket(NetworkMessage message)
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
            
            Index = message.GetByte();
        }
    }
}
