// -----------------------------------------------------------------
// <copyright file="KeepAliveRequestHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Protocol.V772.RequestHandlers
{
    using System.Collections.Generic;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using OpenTibia.Common.Utilities;
    using Serilog;

    /// <summary>
    /// Class that represents a ping request handler for the game server.
    /// </summary>
    public class KeepAliveRequestHandler : BaseRequestHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeepAliveRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger instance.</param>
        public KeepAliveRequestHandler(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForRequestType => (byte)GameRequestType.KeepAlive;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IResponsePacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IResponsePacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            this.Logger.Debug($"Recieved ping back from {connection.SocketIp}.");

            return new PongPacket().YieldSingleItem();
        }
    }
}