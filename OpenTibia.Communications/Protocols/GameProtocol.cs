// -----------------------------------------------------------------
// <copyright file="GameProtocol.cs" company="2Dudes">
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
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Classs that represents the game protocol.
    /// </summary>
    internal class GameProtocol : BaseProtocol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameProtocol"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger to use.</param>
        /// <param name="handlerSelector">A reference to the handler selector to use in this protocol.</param>
        /// <param name="protocolConfigOptions">A reference to the protocol configuration options.</param>
        public GameProtocol(
            ILogger logger,
            IHandlerSelector handlerSelector,
            ProtocolConfigurationOptions protocolConfigOptions)
            : base(handlerSelector)
        {
            logger.ThrowIfNull(nameof(logger));
            protocolConfigOptions.ThrowIfNull(nameof(protocolConfigOptions));

            this.Logger = logger.ForContext<GameProtocol>();
            this.ProtocolConfiguration = protocolConfigOptions;
        }

        /// <summary>
        /// Gets the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets a reference to the Protocol configuration options.
        /// </summary>
        public ProtocolConfigurationOptions ProtocolConfiguration { get; }

        /// <summary>
        /// Processes an incomming message from the connection.
        /// </summary>
        /// <param name="connection">The connection where the message is being read from.</param>
        /// <param name="inboundMessage">The message to process.</param>
        public override void ProcessMessage(IConnection connection, INetworkMessage inboundMessage)
        {
            connection.ThrowIfNull(nameof(connection));
            inboundMessage.ThrowIfNull(nameof(inboundMessage));

            byte packetType;

            if (!connection.IsAuthenticated || connection.XTeaKey.Sum(b => b) == 0)
            {
                // this is a new connection...
                packetType = inboundMessage.GetByte();

                if (packetType != (byte)IncomingGamePacketType.LogIn)
                {
                    // but this is not the packet we were expecting for a new connection.
                    connection.Close();
                    return;
                }

                // Make a copy of the message in case we fail to decrypt using the first set of keys.
                var messageCopy = inboundMessage.Copy();

                inboundMessage.RsaDecrypt(useCipKeys: this.ProtocolConfiguration.UsingCipsoftRsaKeys);

                if (inboundMessage.GetByte() != 0)
                {
                    // means the RSA decrypt was unsuccessful, lets try with the other set of RSA keys...
                    inboundMessage = messageCopy;

                    inboundMessage.RsaDecrypt(useCipKeys: !this.ProtocolConfiguration.UsingCipsoftRsaKeys);

                    if (inboundMessage.GetByte() != 0)
                    {
                        // These RSA keys are also usuccessful... so give up.
                        connection.Close();
                        return;
                    }
                }
            }
            else
            {
                // Decrypt message using XTea
                inboundMessage.XteaDecrypt(connection.XTeaKey);
                inboundMessage.GetUInt16();
                packetType = inboundMessage.GetByte();
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
