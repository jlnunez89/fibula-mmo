// -----------------------------------------------------------------
// <copyright file="IConnection.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Communications.Contracts.Delegates;

    /// <summary>
    /// Interface for all types of connections.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Event fired when this connection has been closed.
        /// </summary>
        event ConnectionClosedDelegate Closed;

        /// <summary>
        /// Event fired right after this connection has proccessed an <see cref="IIncomingPacket"/> by any subscriber of the <see cref="PacketReady"/> event.
        /// </summary>
        event ConnectionPacketProccessedDelegate PacketProcessed;

        /// <summary>
        /// Event fired when a <see cref="IIncomingPacket"/> picked up by this connection is ready to be processed.
        /// </summary>
        event ConnectionPacketReadyDelegate PacketReady;

        /// <summary>
        /// Gets a value indicating whether the connection is an orphan.
        /// </summary>
        bool IsOrphaned { get; }

        /// <summary>
        /// Reads the next message in this connection.
        /// </summary>
        void Read();

        /// <summary>
        /// Closes this connection.
        /// </summary>
        void Close();

        /// <summary>
        /// Sends the packets supplied through this connection.
        /// </summary>
        /// <param name="packetsToSend">The packets to send.</param>
        void Send(IEnumerable<IOutboundPacket> packetsToSend);
    }
}