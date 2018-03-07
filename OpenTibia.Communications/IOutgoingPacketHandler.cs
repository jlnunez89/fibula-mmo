using System.Collections.Generic;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications
{
    public interface INotification
    {
        Connection Connection { get; }

        IList<IPacketOutgoing> ResponsePackets { get; }

        void Prepare();

        void Send();
    }
}
