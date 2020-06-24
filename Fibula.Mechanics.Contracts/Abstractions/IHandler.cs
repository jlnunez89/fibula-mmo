// -----------------------------------------------------------------
// <copyright file="IHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Interface for a request handler.
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// Handles the request contained in a packet.
        /// </summary>
        /// <param name="incomingPacket">The packet to handle.</param>
        /// <param name="client">A reference to the client from where this request originated from, for context.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s that compose that synchronous response, if any.</returns>
        IEnumerable<IOutboundPacket> HandleRequest(IIncomingPacket incomingPacket, IClient client);
    }
}
