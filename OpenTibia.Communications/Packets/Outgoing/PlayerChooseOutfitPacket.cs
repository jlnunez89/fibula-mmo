using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class PlayerChooseOutfitPacket : PacketOutgoing
    {
        public Outfit CurrentOutfit { get; set; }

        public ushort ChooseFromId { get; set; }
        public ushort ChooseToId { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.OutfitWindow;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddUInt16(CurrentOutfit.Id);

            if (CurrentOutfit.Id != 0)
            {
                message.AddByte(CurrentOutfit.Head);
                message.AddByte(CurrentOutfit.Body);
                message.AddByte(CurrentOutfit.Legs);
                message.AddByte(CurrentOutfit.Feet);
            }
            else
            {
                message.AddUInt16(CurrentOutfit.LikeType);
            }

            message.AddUInt16(ChooseFromId);
            message.AddUInt16(ChooseToId);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
