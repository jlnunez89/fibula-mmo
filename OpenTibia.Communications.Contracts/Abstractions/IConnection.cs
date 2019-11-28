// -----------------------------------------------------------------
// <copyright file="IConnection.cs" company="2Dudes">
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
    using OpenTibia.Communications.Contracts.Delegates;

    /// <summary>
    /// Interface for connections.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Event fired when this connection is closed.
        /// </summary>
        event OnConnectionClosed ConnectionClosed;

        /// <summary>
        /// Event fired right after this connection has had it's <see cref="InboundMessage"/> proccessed by any subscriber of the <see cref="MessageReady"/> event.
        /// </summary>
        event OnMessageProccessed MessageProcessed;

        /// <summary>
        /// Event fired when this connection has it's <see cref="InboundMessage"/> ready to be proccessed.
        /// </summary>
        event OnMessageReadyToProccess MessageReady;

        /// <summary>
        /// Gets the inbound message in this connection.
        /// </summary>
        INetworkMessage InboundMessage { get; }

        /// <summary>
        /// Gets the Socket IP address of this connection, if it is open.
        /// </summary>
        string SocketIp { get; }

        /// <summary>
        /// Gets a value indicating whether the connection is an orphan.
        /// </summary>
        bool IsOrphaned { get; }

        /// <summary>
        /// Gets a value indicating whether this connection is authenticated.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets the Id of the player that this connection is associated to.
        /// </summary>
        uint PlayerId { get; }

        /// <summary>
        /// Gets this connection's XTea key.
        /// </summary>
        uint[] XTeaKey { get; }

        /// <summary>
        /// Sets up an Xtea key expected to be matched on subsequent messages.
        /// </summary>
        /// <param name="xteaKey">The XTea key to use in this connection's communications.</param>
        void SetupAuthenticationKey(uint[] xteaKey);

        /// <summary>
        /// Authenticates this connection with the key provided.
        /// </summary>
        /// <param name="xteaKey">The XTea key to validate.</param>
        /// <returns>True if the keys match and the connection is authenticated, false otherwise.</returns>
        bool Authenticate(uint[] xteaKey);

        /// <summary>
        /// Associates this connection with a player.
        /// </summary>
        /// <param name="toPlayerId">The Id of the player that the connection will be associated to.</param>
        void AssociateToPlayer(uint toPlayerId);

        /// <summary>
        /// Begins reading from this connection.
        /// </summary>
        void BeginStreamRead();

        /// <summary>
        /// Closes this connection.
        /// </summary>
        void Close();

        /// <summary>
        /// Sends a network message via this connection.
        /// </summary>
        /// <param name="message">The network message to send.</param>
        void Send(INetworkMessage message);
    }
}