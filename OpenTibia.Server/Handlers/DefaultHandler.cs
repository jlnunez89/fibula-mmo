using System;
using System.Text;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Server.Data;

namespace OpenTibia.Server.Handlers
{
    internal class DefaultHandler : IncomingPacketHandler
    {
        public byte IncomingPacketType { get; }
        
        public DefaultHandler(byte incomingType)
        {
            IncomingPacketType = incomingType;
        }

        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            var debugPacket = new DefaultReadPacket(message);

            var sb = new StringBuilder();

            foreach(var b in debugPacket.InfoBytes)
            {
                sb.AppendFormat("{0:x2} ", b);
            }

            Console.WriteLine($"Default handler received the following packet type: {IncomingPacketType}");
            Console.WriteLine(sb.ToString());
            Console.WriteLine();
        }
    }
}