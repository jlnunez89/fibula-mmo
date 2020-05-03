﻿// -----------------------------------------------------------------
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
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s that compose that synchronous response, if any.</returns>
        IEnumerable<IOutgoingPacket> HandleRequest(INetworkMessage message, IConnection connection);
    }
}
