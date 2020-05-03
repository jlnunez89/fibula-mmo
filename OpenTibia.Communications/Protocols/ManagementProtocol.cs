// -----------------------------------------------------------------
// <copyright file="ManagementProtocol.cs" company="2Dudes">
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
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Classs that represents the management protocol.
    /// </summary>
    internal class ManagementProtocol : BaseProtocol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementProtocol"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger to use.</param>
        /// <param name="handlerSelector">A reference to the handler selector to use in this protocol.</param>
        public ManagementProtocol(ILogger logger, IHandlerSelector handlerSelector)
            : base(handlerSelector)
        {
            logger.ThrowIfNull(nameof(logger));

            this.Logger = logger.ForContext<GameProtocol>();
        }

        /// <summary>
        /// Gets the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Processes an incomming message from the connection.
        /// </summary>
        /// <param name="connection">The connection where the message is being read from.</param>
        /// <param name="inboundMessage">The message to process.</param>
        public override void ProcessMessage(IConnection connection, INetworkMessage inboundMessage)
        {
            IncomingManagementPacketType packetType = (IncomingManagementPacketType)inboundMessage.GetByte();

            // TODO: move this validation?
            if (packetType != IncomingManagementPacketType.AuthenticationRequest && !connection.IsAuthenticated)
            {
                connection.Close();
                return;
            }

            var handler = this.HandlerSelector.SelectForType((byte)packetType);

            if (handler == null)
            {
                return;
            }

            var responsePackets = handler.HandleRequest(inboundMessage, connection);

            if (responsePackets != null && responsePackets.Any())
            {
                var responseMessage = this.PrepareResponse(responsePackets);

                connection.Send(responseMessage);
            }
        }
    }
}
