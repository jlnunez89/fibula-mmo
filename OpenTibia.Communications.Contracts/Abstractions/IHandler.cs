// -----------------------------------------------------------------
// <copyright file="IHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts.Abstractions
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a message handler.
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// Gets the type for which this handler handles messages.
        /// </summary>
        byte ForPacketType { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A value tuple with a value indicating whether the handler intends to respond, and a collection of <see cref="IOutgoingPacket"/>s that compose that response.</returns>
        (bool IntendsToRespond, IEnumerable<IOutgoingPacket> ResponsePackets) HandleRequest(INetworkMessage message, IConnection connection);

        /// <summary>
        /// Prepares a <see cref="INetworkMessage"/> with the reponse packets supplied.
        /// </summary>
        /// <param name="responsePackets">The packets that compose that response.</param>
        /// <returns>The response as a <see cref="INetworkMessage"/>.</returns>
        INetworkMessage PrepareResponse(IEnumerable<IOutgoingPacket> responsePackets);
    }
}
