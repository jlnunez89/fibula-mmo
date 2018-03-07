using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications
{
    internal class ManagementProtocol : OpenTibiaProtocol
    {
        public override bool KeepConnectionOpen => true;

        public ManagementProtocol(IHandlerFactory handlerFactory)
            : base (handlerFactory)
        {

        }

        public override void ProcessPacket(Connection connection, NetworkMessage inboundMessage)
        {
            LoginOrManagementIncomingPacketType packetType = (LoginOrManagementIncomingPacketType)inboundMessage.GetByte();
            
            // TODO: move this validation?
            if(packetType != LoginOrManagementIncomingPacketType.AuthenticationRequest && !connection.IsAuthenticated)
            {
                connection.Close();
                return;
            }

            var handler = HandlerFactory.CreateIncommingForType((byte)packetType);

            handler?.HandlePacket(inboundMessage, connection);

            if (handler?.ResponsePackets != null)
            {
                // Send any responses prepared for this.
                NetworkMessage message = new NetworkMessage();

                foreach (var outPacket in handler.ResponsePackets)
                {
                    message.AddPacket(outPacket);
                }

                connection.Send(message);
            }
        }
    }
}
