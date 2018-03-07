using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class MessageOfTheDayPacket : PacketOutgoing
    {
        public string MessageOfTheDay { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.MessageOfTheDay;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddString("1\n" + MessageOfTheDay);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
