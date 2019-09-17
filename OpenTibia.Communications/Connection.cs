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

    public class Connection : IConnection
    {
        private readonly object writeLock;

        private readonly Socket socket;

        private readonly NetworkStream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="socket">The socket that this connection is for.</param>
        public Connection(Socket socket)
        {
            socket.ThrowIfNull(nameof(socket));

            this.writeLock = new object();
            this.socket = socket;
            this.stream = new NetworkStream(this.socket);

            this.InboundMessage = new NetworkMessage(isOutbound: false);
            this.XTeaKey = new uint[4];
            this.IsAuthenticated = false;
        }

        /// <summary>
        /// Event fired when this connection has been closed.
        /// </summary>
        public event OnConnectionClosed ConnectionClosed;

        /// <summary>
        /// Event fired when this connection has it's <see cref="IConnection.InboundMessage"/> ready to be proccessed.
        /// </summary>
        public event OnMessageReadyToProccess MessageReadyToProccess;

        /// <summary>
        /// Event fired right after this connection has had it's <see cref="IConnection.InboundMessage"/> proccessed by any subscriber of the <see cref="MessageReadyToProccess"/> event.
        /// </summary>
        public event OnMessageProccessed AfterMessageProcessed;

        public INetworkMessage InboundMessage { get; }

        public Guid PlayerId { get; set; }

        public uint[] XTeaKey { get; set; }

        public bool IsAuthenticated { get; set; }

        public string SocketIp
        {
            get
            {
                return this.socket?.RemoteEndPoint?.ToString();
            }
        }

        public bool IsOrphaned
        {
            get
            {
                return !this.socket?.Connected ?? false;
            }
        }

        public void BeginStreamRead()
        {
            this.stream.BeginRead(this.InboundMessage.Buffer, 0, 2, this.OnRead, null);
        }

        public void Send(INetworkMessage message)
        {
            this.Send(message, true);
        }

        public void Send(INetworkMessage message, bool useEncryption, bool managementProtocol = false)
        {
            // if (isInTransaction)
            // {
            //    if (useEncryption == false)
            //        throw new Exception("Cannot send a packet without encryption as part of a transaction.");

            // transactionMessage.AddBytes(message.GetPacket());
            // }
            // else
            // {
            this.SendMessage(message, useEncryption, managementProtocol);
            // }
        }

        public void Send(INotification notification)
        {
            notification.ThrowIfNull(nameof(notification));

            var networkMessage = new NetworkMessage();

            if (notification.Packets.Count == 0)
            {
                return;
            }

            foreach (var packet in notification.Packets)
            {
                packet.WriteToMessage(networkMessage);
            }

            this.Send(networkMessage);

            Console.WriteLine($"Sent {notification.GetType().Name} [{notification.EventId}] to {this.PlayerId}");
        }

        public void Close()
        {
            this.stream.Close();
            this.socket.Close();

            // Tells the subscribers of this event that this connection has been closed.
            this.ConnectionClosed?.Invoke(this);
        }

        private void OnRead(IAsyncResult ar)
        {
            if (!this.CompleteRead(ar))
            {
                return;
            }

            try
            {
                if (this.MessageReadyToProccess != null)
                {
                    this.MessageReadyToProccess.Invoke(this, this.InboundMessage);

                    // By design, AfterMessageProcessed is only fired if we have at least one subscriber.
                    this.AfterMessageProcessed?.Invoke(this);
                }
            }
            catch (Exception e)
            {
                // Invalid data from the client
                // TODO: I must not swallow exceptions.
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                // TODO: is closing the connection really necesary?
                // Close();
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
                // TODO: I must not swallow exceptions.
                // TODO: is closing the connection really necesary?
                Console.WriteLine(e.ToString());
                this.Close();
            }

            return false;
        }

        // private bool isInTransaction = false;
        // private NetworkMessage transactionMessage = new NetworkMessage();

        // public void BeginTransaction()
        // {
        //    if (!isInTransaction)
        //    {
        //        transactionMessage.Reset();
        //        isInTransaction = true;
        //    }
        // }

        // public void CommitTransaction()
        // {
        //    SendMessage(transactionMessage, true);
        //    isInTransaction = false;
        // }

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
