// -----------------------------------------------------------------
// <copyright file="IClient.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Client.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Client.Contracts.Models;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Interface for connections.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Gets a value indicating whether this client is idle.
        /// </summary>
        bool IsIdle { get; }

        /// <summary>
        /// Gets the id of the player that this client is tied to.
        /// </summary>
        public uint PlayerId { get; }

        /// <summary>
        /// Gets the connection enstablished by this client.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// Gets the information about the client on the other side of this connection.
        /// </summary>
        ClientInformation ClientInformation { get; }

        /// <summary>
        /// Sends the packets supplied over the <see cref="Connection"/>.
        /// </summary>
        /// <param name="packetsToSend">The packets to send.</param>
        void Send(IEnumerable<IOutboundPacket> packetsToSend);

        /// <summary>
        /// Associates this connection with a player.
        /// </summary>
        /// <param name="toPlayerId">The Id of the player that the connection will be associated to.</param>
        void AssociateToPlayer(uint toPlayerId);
    }
}