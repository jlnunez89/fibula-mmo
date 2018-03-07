using System;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Server.Data;

namespace OpenTibia.Server.Handlers
{
    internal class DebugAssertionHandler : IncomingPacketHandler
    {
        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            var packet = new DebugAssertionPacket(message);

            Console.WriteLine($"{packet.AssertionLine}");
            Console.WriteLine($"{packet.Date}");
            Console.WriteLine($"{packet.Description}");
            Console.WriteLine($"{packet.Comment}");
        }
    }
}