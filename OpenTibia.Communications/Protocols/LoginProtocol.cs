// -----------------------------------------------------------------
// <copyright file="LoginProtocol.cs" company="2Dudes">
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
    using System.Diagnostics;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    /// <summary>
    /// Classs that represents the login protocol.
    /// </summary>
    internal class LoginProtocol : BaseProtocol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginProtocol"/> class.
        /// </summary>
        /// <param name="handlerSelector">A reference to the handler selector to use in this protocol.</param>
        /// <param name="protocolConfigOptions">A reference to the protocol configuration options.</param>
        /// <param name="gameConfigOptions">A reference to the game configuration options.</param>
        public LoginProtocol(
            IHandlerSelector handlerSelector,
            ProtocolConfigurationOptions protocolConfigOptions,
            GameConfigurationOptions gameConfigOptions)
            : base(handlerSelector, keepConnectionOpen: false)
        {
            protocolConfigOptions.ThrowIfNull(nameof(protocolConfigOptions));
            gameConfigOptions.ThrowIfNull(nameof(gameConfigOptions));

            this.ProtocolConfiguration = protocolConfigOptions;
            this.GameConfiguration = gameConfigOptions;
        }

        /// <summary>
        /// Gets a reference to the protocol configuration options.
        /// </summary>
        public ProtocolConfigurationOptions ProtocolConfiguration { get; }

        /// <summary>
        /// Gets a reference to the game configuration options.
        /// </summary>
        public GameConfigurationOptions GameConfiguration { get; }

        /// <summary>
        /// Processes an incomming message from the connection.
        /// </summary>
        /// <param name="connection">The connection where the message is being read from.</param>
        /// <param name="inboundMessage">The message to process.</param>
        public override void ProcessMessage(IConnection connection, INetworkMessage inboundMessage)
        {
            connection.ThrowIfNull(nameof(connection));
            inboundMessage.ThrowIfNull(nameof(inboundMessage));

            byte packetType = inboundMessage.GetByte();

            if (packetType != (byte)IncomingManagementPacketType.LoginServerRequest)
            {
                // This packet should NOT have been routed to this protocol.
                // TODO: proper logging.
                Trace.TraceWarning($"Non {nameof(IncomingManagementPacketType.LoginServerRequest)} packet routed to {nameof(LoginProtocol)}. Packet was ignored.");
                return;
            }

            var handler = this.HandlerSelector.SelectForType(packetType);

            if (handler == null)
            {
                return;
            }

            var (intendsToRespond, responsePackets) = handler.HandleRequest(inboundMessage, connection);

            if (intendsToRespond)
            {
                // Send any responses prepared for this.
                var responseMessage = handler.PrepareResponse(responsePackets);

                connection.Send(responseMessage);
            }
        }
    }
}
