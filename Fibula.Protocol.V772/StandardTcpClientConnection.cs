// -----------------------------------------------------------------
// <copyright file="StandardTcpClientConnection.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Protocol.V772
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using Fibula.Common.Utilities;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Delegates;
    using Fibula.Communications.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents a standard client connection over a TCP socket.
    /// </summary>
    public class StandardTcpClientConnection : ISocketConnection
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
        /// Gets a reference to the inbound network message.
        /// </summary>
        private readonly INetworkMessage inboundMessage;

        /// <summary>
        /// Gets a reference to the logger in use.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Stores the protocol in use by this connection.
        /// </summary>
        private readonly IProtocol protocol;

        /// <summary>
        /// A value indicating whether this connection has been authenticated.
        /// </summary>
        private bool isAuthenticated;

        /// <summary>
        /// The XTea key used for all communications through this connection.
        /// </summary>
        private uint[] xteaKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardTcpClientConnection"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="socket">The socket that this connection is for.</param>
        /// <param name="protocol">The protocol in use by this connection.</param>
        public StandardTcpClientConnection(ILogger logger, Socket socket, IProtocol protocol)
        {
            logger.ThrowIfNull(nameof(logger));
            socket.ThrowIfNull(nameof(socket));
            protocol.ThrowIfNull(nameof(protocol));

            this.writeLock = new object();
            this.socket = socket;
            this.stream = new NetworkStream(this.socket);

            this.inboundMessage = new NetworkMessage(isOutbound: false);
            this.xteaKey = new uint[4];
            this.isAuthenticated = false;

            this.logger = logger.ForContext<StandardTcpClientConnection>();
            this.protocol = protocol;
        }

        /// <summary>
        /// Event fired when this connection has been closed.
        /// </summary>
        public event ConnectionClosedDelegate Closed;

        /// <summary>
        /// Event fired right after this connection has proccessed an <see cref="IIncomingPacket"/> by any subscriber of the <see cref="PacketReady"/> event.
        /// </summary>
        public event ConnectionPacketProccessedDelegate PacketProcessed;

        /// <summary>
        /// Event fired when a <see cref="IIncomingPacket"/> picked up by this connection is ready to be processed.
        /// </summary>
        public event ConnectionPacketReadyDelegate PacketReady;

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
        /// Reads from this connection's stream.
        /// </summary>
        public void Read()
        {
            var lenArray = new byte[2];
            this.stream.BeginRead(lenArray, 0, 2, this.OnDataReady, null);
        }

        /// <summary>
        /// Closes this connection.
        /// </summary>
        public void Close()
        {
            this.stream.Close();
            this.socket.Close();

            // Tells the subscribers of this event that this connection has been closed.
            this.Closed?.Invoke(this);
        }

        /// <summary>
        /// Prepares a <see cref="INetworkMessage"/> with the reponse packets supplied and sends it.
        /// </summary>
        /// <param name="responsePackets">The packets that compose that response.</param>
        public void Send(IEnumerable<IOutboundPacket> responsePackets)
        {
            if (responsePackets == null || !responsePackets.Any() || this.IsOrphaned)
            {
                return;
            }

            INetworkMessage outboundMessage = new NetworkMessage();

            int readAlready = 4; // 4 to skip the length bytes.

            foreach (var outPacket in responsePackets)
            {
                var writer = this.protocol.SelectPacketWriter(outPacket.PacketType);

                if (writer == null)
                {
                    this.logger.Warning($"Unsupported response packet type {outPacket.PacketType} without a writer. Packet was not added to the message.");

                    continue;
                }

                writer.WriteToMessage(outPacket, ref outboundMessage);

                var thisPacketLen = outboundMessage.Length - readAlready;
                var packetBytes = outboundMessage.Buffer.Slice(readAlready, thisPacketLen)
                                                        .ToArray()
                                                        .Select(b => b.ToString("X2")).Aggregate((str, e) => str += " " + e);

                this.logger.Verbose($"Message bytes added by packet {outPacket.GetType().Name}: {packetBytes}");

                readAlready += thisPacketLen;
            }

            this.Send(outboundMessage);
        }

        /// <summary>
        /// Sets up an Xtea key expected to be matched on subsequent messages.
        /// </summary>
        /// <param name="xteaKey">The XTea key to use in this connection's communications.</param>
        public void SetupAuthenticationKey(uint[] xteaKey)
        {
            this.xteaKey = xteaKey;
            this.isAuthenticated = true;
        }

        private void OnDataReady(IAsyncResult ar)
        {
            if (!this.CompleteRead(ar))
            {
                return;
            }

            try
            {
                if (this.stream.CanRead)
                {
                    if (this.stream.DataAvailable)
                    {
                        var msgSize = this.stream.Read(this.inboundMessage.Buffer);

                        this.inboundMessage.Resize(msgSize);

                        if (this.isAuthenticated)
                        {
                            // Decrypt message using XTea
                            this.inboundMessage.XteaDecrypt(this.xteaKey);

                            // Read the total length.
                            this.inboundMessage.GetUInt16();
                        }

                        var packetType = this.inboundMessage.GetByte();
                        var reader = this.protocol.SelectPacketReader(packetType);

                        if (reader == null)
                        {
                            if (Enum.IsDefined(typeof(IncomingGamePacketType), packetType))
                            {
                                this.logger.Warning($"No reader found that supports type '{(IncomingGamePacketType)packetType}' of packets. Selecting default reader...");
                            }
                            else if (Enum.IsDefined(typeof(IncomingGatewayPacketType), packetType))
                            {
                                this.logger.Warning($"No reader found that supports type '{(IncomingGatewayPacketType)packetType}' of packets. Selecting default reader...");
                            }
                            else
                            {
                                this.logger.Warning($"No reader found that supports type '{packetType}' of packets. Selecting default reader...");
                            }

                            reader = new DefaultPacketReader(this.logger);
                        }

                        var dataRead = reader.ReadFromMessage(this.inboundMessage);

                        if (dataRead == null)
                        {
                            this.logger.Error($"Could not read data using reader '{reader.GetType().Name}'.");
                        }
                        else
                        {
                            var responsePackets = this.PacketReady?.Invoke(this, dataRead);

                            if (responsePackets != null && responsePackets.Any())
                            {
                                // Send any response packets prepared.
                                this.Send(responsePackets);
                            }
                        }

                        this.inboundMessage.Reset();
                    }

                    this.PacketProcessed?.Invoke(this);
                }
            }
            catch (Exception e)
            {
                // Invalid data from the client
                this.logger.Warning(e.ToString());
            }
        }

        private bool CompleteRead(IAsyncResult ar)
        {
            try
            {
                int read = this.stream.CanRead ? this.stream.EndRead(ar) : 0;

                if (read == 0)
                {
                    // client disconnected
                    this.Close();

                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                this.logger.Error(e.ToString());

                // TODO: is closing the connection really necesary?
                this.Close();
            }

            return false;
        }

        private void Send(INetworkMessage message)
        {
            message.PrepareToSend(this.xteaKey);

            try
            {
                lock (this.writeLock)
                {
                    // limit to the prepared length of the message only.
                    var spanToSend = message.Buffer[0..message.Length];

                    this.stream.Write(spanToSend);
                }
            }
            catch (ObjectDisposedException e)
            {
                this.logger.Error(e.ToString());

                this.Close();
            }
        }
    }
}
