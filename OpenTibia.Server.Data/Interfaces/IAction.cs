using System.Collections.Generic;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Data.Interfaces
{
    public interface IAction
    {
        IPacketIncoming Packet { get; }

        Location RetryLocation { get; }

        IList<IPacketOutgoing> ResponsePackets { get; }

        void Perform();
    }
}