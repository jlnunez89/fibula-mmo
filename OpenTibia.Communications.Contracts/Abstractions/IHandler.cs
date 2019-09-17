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
        /// Gets a value indicating whether this handler intends to respond to the request.
        /// </summary>
        bool IntendsToRespond { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        void HandleRequest(INetworkMessage message, IConnection connection);

        /// <summary>
        /// Prepares a <see cref="INetworkMessage"/> as a response if this handler <see cref="IntendsToRespond"/>.
        /// </summary>
        /// <returns>The response as a <see cref="INetworkMessage"/>.</returns>
        INetworkMessage PrepareResponse();
    }
}
