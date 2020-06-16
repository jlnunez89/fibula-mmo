// -----------------------------------------------------------------
// <copyright file="TcpClient.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Models;

    /// <summary>
    /// Class that implements an <see cref="IClient"/> for Tcp connections.
    /// </summary>
    public class TcpClient : IClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClient"/> class.
        /// </summary>
        /// <param name="connection">The connection that this client uses.</param>
        public TcpClient(IConnection connection)
        {
            connection.ThrowIfNull(nameof(connection));

            if (!(connection is ISocketConnection socketConnection))
            {
                throw new ArgumentException($"This client requires a {nameof(connection)} derived of type {nameof(ISocketConnection)}.");
            }

            this.Connection = socketConnection;
            this.ClientInformation = new ClientInformation()
            {
                Agent = "Unknown",
                Version = "Unknown",
            };
        }

        /// <summary>
        /// Gets a value indicating whether this client is idle.
        /// </summary>
        public bool IsIdle { get; }

        /// <summary>
        /// Gets the connection enstablished by this client.
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        /// Gets the information about the client on the other side of this connection.
        /// </summary>
        public ClientInformation ClientInformation { get; }

        /// <summary>
        /// Sends the packets supplied over the <see cref="Connection"/>.
        /// </summary>
        /// <param name="packetsToSend">The packets to send.</param>
        public void Send(IEnumerable<IOutboundPacket> packetsToSend)
        {
            if (packetsToSend == null || !packetsToSend.Any())
            {
                return;
            }

            this.Connection.Send(packetsToSend);
        }
    }
}
