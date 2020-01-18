// -----------------------------------------------------------------
// <copyright file="KeepAliveRequestHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Handlers.Game
{
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets.Outgoing;
    using Serilog;

    /// <summary>
    /// Class that represents a ping request handler for the game server.
    /// </summary>
    public class KeepAliveRequestHandler : BaseHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeepAliveRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger instance.</param>
        public KeepAliveRequestHandler(ILogger logger)
        {
            logger.ThrowIfNull(nameof(logger));

            this.Logger = logger.ForContext<KeepAliveRequestHandler>();
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.KeepAlive;

        /// <summary>
        /// Gets the reference to the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A value tuple with a value indicating whether the handler intends to respond, and a collection of <see cref="IOutgoingPacket"/>s that compose that response.</returns>
        public override (bool IntendsToRespond, IEnumerable<IOutgoingPacket> ResponsePackets) HandleRequest(INetworkMessage message, IConnection connection)
        {
            this.Logger.Debug($"Recieved ping back from {connection.SocketIp}.");

            var responsePackets = new List<IOutgoingPacket>
            {
                new PongPacket(),
            };

            return (true, responsePackets);
        }
    }
}