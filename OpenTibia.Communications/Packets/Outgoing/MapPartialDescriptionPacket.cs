using System;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class MapPartialDescriptionPacket : PacketOutgoing
    {
        public override byte PacketType { get; }

        public byte[] DescriptionBytes { get; set; }

        public MapPartialDescriptionPacket(GameOutgoingPacketType mapDescriptionType)
        {
            if (mapDescriptionType != GameOutgoingPacketType.MapSliceEast  &&
                mapDescriptionType != GameOutgoingPacketType.MapSliceNorth &&
                mapDescriptionType != GameOutgoingPacketType.MapSliceSouth &&
                mapDescriptionType != GameOutgoingPacketType.MapSliceWest  &&
                mapDescriptionType != GameOutgoingPacketType.FloorChangeUp &&
                mapDescriptionType != GameOutgoingPacketType.FloorChangeDown)
            {
                throw new ArgumentException(nameof(mapDescriptionType));
            }

            PacketType = (byte)mapDescriptionType;
        }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddBytes(DescriptionBytes);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
