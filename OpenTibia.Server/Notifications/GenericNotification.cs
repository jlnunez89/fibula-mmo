using System;
using System.Collections.Generic;
using System.Linq;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Outgoing;

namespace OpenTibia.Server.Notifications
{
    internal class GenericNotification : Notification
    {
        public IEnumerable<PacketOutgoing> OutgoingPackets { get; }

        public GenericNotification(Connection connection, params PacketOutgoing[] outgoingPackets)  
            : base(connection)
        {
            if (outgoingPackets == null || !outgoingPackets.Any())
            {
                throw new ArgumentNullException(nameof(outgoingPackets));
            }

            OutgoingPackets = outgoingPackets;
        }

        public override void Prepare()
        {
            foreach(var packet in OutgoingPackets)
            {
                ResponsePackets.Add(packet);
            }
        }
    }
}