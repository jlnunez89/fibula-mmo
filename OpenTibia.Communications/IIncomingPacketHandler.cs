using System.Collections.Generic;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications
{
    public interface IIncomingPacketHandler
    {
        /// <summary>
        /// A collection of <see cref="IPacketOutgoing"/> packets readily available to be sent as response when this message was handled.
        /// The handler may or may not add contents to this collection. The collection may also be null.
        /// </summary>
        IList<IPacketOutgoing> ResponsePackets { get; }

        /// <summary>
        /// Handles the contents of <paramref name="message"/>. 
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message came, for context.</param>
        /// <returns>True if the message was handled satisfactorily, False otherwise.</returns>
        void HandlePacket(NetworkMessage message, Connection connection);
    }
}
