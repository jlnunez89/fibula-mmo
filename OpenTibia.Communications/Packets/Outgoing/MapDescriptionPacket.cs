using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class MapDescriptionPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.MapDescription;

        public Location Origin { get; set; }

        public byte[] DescriptionBytes { get; set; }
        
        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddLocation(Origin);
            
            message.AddBytes(DescriptionBytes); // TODO: change this?
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
