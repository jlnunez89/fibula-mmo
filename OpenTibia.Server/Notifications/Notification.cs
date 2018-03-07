using System;
using System.Collections.Generic;
using System.Linq;
using OpenTibia.Communications;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Notifications
{
    public abstract class Notification : INotification
    {
        public Connection Connection {  get; }

        public IList<IPacketOutgoing> ResponsePackets { get; protected set; }

        protected Notification(Connection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            Connection = connection;
            ResponsePackets = new List<IPacketOutgoing>();
        }

        protected Notification(Connection connection, params IPacketOutgoing[] packets)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            
            Connection = connection;

            foreach (var packet in packets)
            {
                ResponsePackets.Add(packet);
            }
        }

        public abstract void Prepare();

        public void Send()
        {
            if (!ResponsePackets.Any())
            {
                return;
            }

            var networkMessage = new NetworkMessage(4);
                
            foreach(var packet in ResponsePackets)
            {
                networkMessage.AddPacket(packet);
            }

            Connection.Send(networkMessage);

            //foreach (var packet in ResponsePackets)
            //{
            //    packet.CleanUp();
            //}
        }
    }
}
