using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class BanismentResultPacket : PacketOutgoing
    {
        public byte BanDays { get; set; }
        public uint BanishedUntil { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(0x00);
            message.AddByte(BanDays);
            message.AddUInt32(BanishedUntil);
            message.AddByte(0x00);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}