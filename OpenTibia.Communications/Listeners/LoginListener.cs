// -----------------------------------------------------------------
// <copyright file="LoginListener.cs" company="2Dudes">
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
    using Microsoft.Extensions.Options;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Security.Contracts;
    using Serilog;

    /// <summary>
    /// Class that extends the standard <see cref="BaseListener"/> for the login protocol.
    /// </summary>
    /// <typeparam name="THandlerSelector">The type of handler selector in use here.</typeparam>
    public class LoginListener<THandlerSelector> : BaseListener
        where THandlerSelector : class, IHandlerSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginListener{THandlerSelector}"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="options">The options for this listener.</param>
        /// <param name="handlerSelector">A reference to the handler selector to use in this protocol.</param>
        /// <param name="dosDefender">A reference to the DoS defender service implementation.</param>
        public LoginListener(ILogger logger, IOptions<LoginListenerOptions> options, THandlerSelector handlerSelector, IDoSDefender dosDefender)
            : base(logger, options.Value, handlerSelector, dosDefender, keepConnectionOpen: false)
        {
        }

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
                this.Logger.Warning($"Non {nameof(IncomingManagementPacketType.LoginServerRequest)} packet routed to {nameof(LoginListener<THandlerSelector>)}. Packet was ignored.");

                return;
            }

            var handler = this.HandlerSelector.SelectForType(packetType);

            if (handler == null)
            {
                return;
            }

            var responsePackets = handler.HandleRequest(inboundMessage, connection);

            if (responsePackets != null && responsePackets.Any())
            {
                // Send any responses prepared for this.
                var responseMessage = this.PrepareResponse(responsePackets);

                connection.Send(responseMessage);
            }
        }
    }
}
