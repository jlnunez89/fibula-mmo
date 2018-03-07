using System;
using OpenTibia.Server.Data;

namespace OpenTibia.Communications
{
    public enum OpenTibiaProtocolType
    {
        LoginProtocol,
        GameProtocol,
        ManagementProtocol
    }

    public interface IProtocol
    {
        bool KeepConnectionOpen { get; }

        void ProcessPacket(Connection connection, NetworkMessage inboundMessage);

        void OnAcceptNewConnection(Connection connection, IAsyncResult ar);

        void PostProcessPacket(Connection connection);
    }
}
