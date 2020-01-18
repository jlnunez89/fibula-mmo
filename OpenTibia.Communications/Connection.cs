// -----------------------------------------------------------------
// <copyright file="Connection.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications
{
    using System;
    using System.Net.Sockets;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Delegates;
    using Serilog;

    /// <summary>
    /// Class that represents a connection.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is done through ThrowIfNull* methods.")]
    public class Connection : IConnection
    {
        /// <summary>
        /// A lock used to sempahore writes to the connection's stream.
        /// </summary>
        private readonly object writeLock;

        /// <summary>
        /// The socket of this connection.
        /// </summary>
        private readonly Socket socket;

        /// <summary>
        /// This connection's stream.
        /// </summary>
        private readonly NetworkStream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="socket">The socket that this connection is for.</param>
        public Connection(ILogger logger, Socket socket)
        {
            logger.ThrowIfNull(nameof(logger));
            socket.ThrowIfNull(nameof(socket));

            this.writeLock = new object();
            this.socket = socket;
            this.stream = new NetworkStream(this.socket);

            this.InboundMessage = new NetworkMessage(isOutbound: false);
            this.XTeaKey = new uint[4];
            this.IsAuthenticated = false;

            this.Logger = logger.ForContext<Connection>();
        }

        /// <summary>
        /// Event fired when this connection has been closed.
        /// </summary>
        public event OnConnectionClosed ConnectionClosed;

        /// <summary>
        /// Event fired when this connection has it's <see cref="IConnection.InboundMessage"/> ready to be proccessed.
        /// </summary>
        public event OnMessageReadyToProccess MessageReady;

        /// <summary>
        /// Event fired right after this connection has had it's <see cref="IConnection.InboundMessage"/> proccessed by any subscriber of the <see cref="MessageReady"/> event.
        /// </summary>
        public event OnMessageProccessed MessageProcessed;

        /// <summary>
        /// Gets a reference to the inbound network message.
        /// </summary>
        public INetworkMessage InboundMessage { get; }

        /// <summary>
        /// Gets the XTea key used for all communications through this connection.
        /// </summary>
        public uint[] XTeaKey { get; private set; }

        /// <summary>
        /// Gets the id of the player that this connection is tied to.
        /// </summary>
        public uint PlayerId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this connection has been authenticated.
        /// </summary>
        public bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Gets a reference to the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the Socket IP address of this connection, if it is open.
        /// </summary>
        public string SocketIp
        {
            get
            {
                return this.socket?.RemoteEndPoint?.ToString();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the connection is an orphan.
        /// </summary>
        public bool IsOrphaned
        {
            get
            {
                return !this.socket?.Connected ?? false;
            }
        }

        /// <summary>
        /// Begins reading from this connection.
        /// </summary>
        public void BeginStreamRead()
        {
            this.stream.BeginRead(this.InboundMessage.Buffer, 0, 2, this.OnRead, null);
        }

        /// <summary>
        /// Closes this connection.
        /// </summary>
        public void Close()
        {
            this.stream.Close();
            this.socket.Close();

            // Tells the subscribers of this event that this connection has been closed.
            this.ConnectionClosed?.Invoke(this);
        }

        /// <summary>
        /// Sends a network message via this connection.
        /// </summary>
        /// <param name="message">The network message to send.</param>
        public void Send(INetworkMessage message)
        {
            this.Send(message, true);
        }

        /// <summary>
        /// Associates this connection with a player.
        /// </summary>
        /// <param name="toPlayerId">The Id of the player that the connection will be associated to.</param>
        public void AssociateToPlayer(uint toPlayerId)
        {
            this.PlayerId = toPlayerId;
        }

        /// <summary>
        /// Authenticates this connection with the key provided to it.
        /// </summary>
        /// <param name="xteaKey">The XTea key to use in this connection's communications.</param>
        /// <returns>True if the keys match and the connection is authenticated, false otherwise.</returns>
        public bool Authenticate(uint[] xteaKey)
        {
            this.IsAuthenticated = this.XTeaKey.Length == xteaKey.Length;

            for (int i = 0; i < this.XTeaKey.Length; i++)
            {
                this.IsAuthenticated &= this.XTeaKey[i] == xteaKey[i];
            }

            return this.IsAuthenticated;
        }

        /// <summary>
        /// Sets up an Xtea key expected to be matched on subsequent messages.
        /// </summary>
        /// <param name="xteaKey">The XTea key to use in this connection's communications.</param>
        public void SetupAuthenticationKey(uint[] xteaKey)
        {
            this.XTeaKey = xteaKey;
        }

        private void OnRead(IAsyncResult ar)
        {
            if (!this.CompleteRead(ar))
            {
                return;
            }

            try
            {
                if (this.MessageReady != null)
                {
                    this.MessageReady.Invoke(this, this.InboundMessage);

                    // By design, AfterMessageProcessed is only fired if we have at least one subscriber.
                    this.MessageProcessed?.Invoke(this);
                }
            }
            catch (Exception e)
            {
                // Invalid data from the client
                this.Logger.Warning(e.Message);
                this.Logger.Warning(e.StackTrace);
            }
        }

        private bool CompleteRead(IAsyncResult ar)
        {
            try
            {
                int read = this.stream.EndRead(ar);

                if (read == 0)
                {
                    // client disconnected
                    this.Close();

                    return false;
                }

                int size = BitConverter.ToUInt16(this.InboundMessage.Buffer, 0) + 2;

                while (read < size)
                {
                    if (this.stream.CanRead)
                    {
                        read += this.stream.Read(this.InboundMessage.Buffer, read, size - read);
                    }
                }

                this.InboundMessage.Resize(size);
                this.InboundMessage.GetUInt16(); // total length

                return true;
            }
            catch (Exception e)
            {
                this.Logger.Error(e.ToString());

                // TODO: is closing the connection really necesary?
                this.Close();
            }

            return false;
        }

        private void Send(INetworkMessage message, bool useEncryption, bool managementProtocol = false)
        {
            this.SendMessage(message, useEncryption, managementProtocol);
        }

        private void SendMessage(INetworkMessage message, bool useEncryption, bool managementProtocol = false)
        {
            if (useEncryption)
            {
                message.PrepareToSend(this.XTeaKey);
            }
            else
            {
                message.PrepareToSendWithoutEncryption(managementProtocol);
            }

            try
            {
                lock (this.writeLock)
                {
                    this.stream.BeginWrite(message.Buffer, 0, message.Length, null, null);
                }
            }
            catch (ObjectDisposedException)
            {
                this.Close();
            }
        }
    }
}
