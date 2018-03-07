using System;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class TextMessagePacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.TextMessage;

        public MessageType Type { get; set; }

        public string Message { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddByte((byte)Type);
            message.AddString(Message);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
