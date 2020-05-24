// -----------------------------------------------------------------
// <copyright file="BaseListener.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Security.Contracts;
    using Serilog;

    /// <summary>
    /// Class that is the base implementation for all listeners.
    /// </summary>
    public abstract class BaseListener : TcpListener, IListener
    {
        /// <summary>
        /// The DoS defender to use.
        /// </summary>
        private readonly IDoSDefender dosDefender;

        /// <summary>
        /// A value indicating whether the protocol should keep the connection open after recieving a packet.
        /// </summary>
        private readonly bool keepConnectionOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseListener"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="options">The options for this listener.</param>
        /// <param name="handlerSelector">A reference to the handler selector to use in this protocol.</param>
        /// <param name="dosDefender">A reference to a DoS defender service implementation.</param>
        /// <param name="keepConnectionOpen">Optional. A value indicating whether to maintain the connection open after processing a message in the connection.</param>
        protected BaseListener(ILogger logger, BaseListenerOptions options, IHandlerSelector handlerSelector, IDoSDefender dosDefender, bool keepConnectionOpen = true)
            : base(IPAddress.Any, options.Port)
        {
            logger.ThrowIfNull(nameof(logger));

            Validator.ValidateObject(options, new ValidationContext(options), validateAllProperties: true);

            this.dosDefender = dosDefender;
            this.keepConnectionOpen = keepConnectionOpen;

            this.HandlerSelector = handlerSelector;
            this.Logger = logger.ForContext(this.GetType());
        }

        /// <summary>
        /// Gets the logger in use.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the handler selector in use.
        /// </summary>
        protected IHandlerSelector HandlerSelector { get; }

        /// <summary>
        /// Begins listening for requests.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous listening operation.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                try
                {
                    this.Start();

                    // Stop() makes AcceptSocketAsync() throw an ObjectDisposedException.
                    // This means that when the token is cancelled, the callback action here will be to Stop() the listener,
                    // which in turn throws the exception and it gets caught below, exiting gracefully.
                    cancellationToken.Register(() => this.Stop());

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var socket = await this.AcceptSocketAsync().ConfigureAwait(false);

                            Connection connection = new Connection(this.Logger, socket);

                            if (this.dosDefender?.IsBlocked(connection.SocketIp) ?? false)
                            {
                                // TODO: evaluate if it is worth just leaving the connection open but ignore it, so that they think they are successfully DoSing...
                                // But we would need to think if it is a connection drain attack then...
                                connection.Close();

                                continue;
                            }

                            connection.ConnectionClosed += this.OnConnectionClose;
                            connection.MessageReady += this.ProcessMessage;
                            connection.MessageProcessed += this.PostProcessMessage;

                            this.dosDefender?.LogConnectionAttempt(connection.SocketIp);

                            connection.BeginStreamRead();
                        }
                        catch (ObjectDisposedException)
                        {
                            // This is normal when the listerner is stopped because of token cancellation.
                            break;
                        }
                    }
                }
                catch (SocketException socEx)
                {
                    this.Logger.Error(socEx.ToString());
                }
            });

            // return this to allow other IHostedService-s to start.
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the listener.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.Stop();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Processes an inbound message from the connection.
        /// </summary>
        /// <param name="connection">The connection where the message is being read from.</param>
        /// <param name="inboundMessage">The message to process.</param>
        public abstract void ProcessMessage(IConnection connection, INetworkMessage inboundMessage);

        /// <summary>
        /// Runs after processing a message from the connection.
        /// </summary>
        /// <param name="connection">The connection where the message is from.</param>
        public virtual void PostProcessMessage(IConnection connection)
        {
            if (!this.keepConnectionOpen)
            {
                connection.Close();
                return;
            }

            connection.InboundMessage.Reset();
            connection.BeginStreamRead();
        }

        /// <summary>
        /// Prepares a <see cref="INetworkMessage"/> with the reponse packets supplied.
        /// </summary>
        /// <param name="responsePackets">The packets that compose that response.</param>
        /// <returns>The response as a <see cref="INetworkMessage"/>.</returns>
        protected INetworkMessage PrepareResponse(IEnumerable<IOutgoingPacket> responsePackets)
        {
            if (responsePackets == null || !responsePackets.Any())
            {
                return null;
            }

            INetworkMessage outgoingMessage = new NetworkMessage();

            foreach (var outPacket in responsePackets)
            {
                outPacket.WriteToMessage(outgoingMessage);
            }

            return outgoingMessage;
        }

        /// <summary>
        /// Handles a connection's close event.
        /// </summary>
        /// <param name="connection">The connection that was closed.</param>
        private void OnConnectionClose(IConnection connection)
        {
            if (connection == null)
            {
                // TODO: log a warning here.
                return;
            }

            // De-subscribe to this event first.
            connection.ConnectionClosed -= this.OnConnectionClose;
            connection.MessageReady -= this.ProcessMessage;
            connection.MessageProcessed -= this.PostProcessMessage;
        }
    }
}
