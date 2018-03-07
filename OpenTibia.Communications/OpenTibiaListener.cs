using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace OpenTibia.Communications
{
    public abstract class OpenTibiaListener : TcpListener, IOpenTibiaListener
    {
        public IProtocol Protocol { get; }
        public int Port { get; }
        protected ICollection<Connection> Connections { get; }

        protected OpenTibiaListener(int port, IProtocol protocol)
            : base(IPAddress.Any, port)
        {
            if(protocol == null)
            {
                throw new ArgumentNullException(nameof(protocol));
            }

            Port = port;
            Protocol = protocol;
            Connections = new HashSet<Connection>();
        }

        public void BeginListening()
        {
            Start();
            BeginAcceptSocket(OnAccept, this);
        }

        public void EndListening()
        {
            Stop();
        }

        public void OnAccept(IAsyncResult ar)
        {
            Connection connection = new Connection();

            connection.OnCloseEvent += OnConnectionClose;
            connection.OnProcessEvent += Protocol.ProcessPacket;
            connection.OnPostProcessEvent += Protocol.PostProcessPacket;

            Connections.Add(connection);
            Protocol.OnAcceptNewConnection(connection, ar);

            BeginAcceptSocket(OnAccept, this);
        }

        private void OnConnectionClose(Connection connection)
        {
            // De-subscribe to this event first.
            connection.OnCloseEvent -= OnConnectionClose;
            connection.OnProcessEvent -= Protocol.ProcessPacket;
            connection.OnPostProcessEvent -= Protocol.PostProcessPacket;

            Connections.Remove(connection);
        }
    }
}
