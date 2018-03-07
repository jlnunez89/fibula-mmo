using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class DebugAssertionPacket : PacketIncoming
    {
        public string AssertionLine { get; private set; }
        public string Date { get; private set; }
        public string Description { get; private set; }
        public string Comment { get; private set; }

        public DebugAssertionPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            AssertionLine = message.GetString();
            Date = message.GetString();
            Description = message.GetString();
            Comment = message.GetString();
        }
    }
}
