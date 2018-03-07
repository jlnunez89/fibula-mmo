using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class OutfitChangedPacket : PacketIncoming
    {
        public Outfit Outfit { get; set; }

        public OutfitChangedPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {            
            ushort lookType = message.GetUInt16();

            if (lookType != 0)
            {
                Outfit = new Outfit
                {
                    Id = lookType,
                    Head = message.GetByte(),
                    Body = message.GetByte(),
                    Legs = message.GetByte(),
                    Feet = message.GetByte()
                };
            }
            else
            {
                Outfit = new Outfit
                {
                    Id = lookType,
                    LikeType = message.GetUInt16()
                };
            }
        }
    }
}
