using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class LookAtPacket : PacketIncoming
    {
        public Location Location { get; private set; }
        public ushort ThingId { get; private set; }
        public byte StackPosition { get; private set; }

        public LookAtPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            Location = new Location
            {
                X = message.GetUInt16(),
                Y = message.GetUInt16(),
                Z = (sbyte)message.GetByte()
            };

            ThingId = message.GetUInt16();
            StackPosition = message.GetByte();
        }
    }
}
