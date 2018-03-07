using System;
using OpenTibia.Server.Data;

namespace OpenTibia.Communications
{
    public abstract class OpenTibiaProtocol : IProtocol
    {
        public virtual bool KeepConnectionOpen { get; protected set; }

        public IHandlerFactory HandlerFactory { get; protected set; }

        protected OpenTibiaProtocol(IHandlerFactory handlerFactory)
        {
            if ( handlerFactory == null)
            {
                throw new ArgumentNullException(nameof(handlerFactory));
            }

            HandlerFactory = handlerFactory;
        }

        public virtual void OnAcceptNewConnection(Connection connection, IAsyncResult ar)
        {
            if(connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            connection.OnAccept(ar);
        }

        public virtual void PostProcessPacket(Connection connection)
        {
            if(!KeepConnectionOpen)
            {
                connection.Close();
            }
            else if (connection.Stream != null)
            {
                connection.InMessage.Reset();
                connection.BeginStreamRead();
            }
        }

        public abstract void ProcessPacket(Connection connection, NetworkMessage inboundMessage);
    }
}
