// -----------------------------------------------------------------
// <copyright file="DefaultHandler.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using System.Text;
    using OpenTibia.Communications.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Special kind of handler that is used as a fall back when no other handler is picked.
    /// </summary>
    public class DefaultHandler : BaseHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="packetType">The type of the packet.</param>
        public DefaultHandler(ILogger logger, byte packetType)
            : base(logger)
        {
            this.IncomingPacketType = packetType;
        }

        /// <summary>
        /// Gets the packet type that resulted in this handler being picked.
        /// </summary>
        public byte IncomingPacketType { get; }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => default;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutgoingPacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            var debugInfo = message.ReadDefaultInfo();

            var sb = new StringBuilder();

            foreach (var b in debugInfo.Bytes)
            {
                sb.AppendFormat("{0:x2} ", b);
            }

            this.Logger.Information($"Default handler received the following packet type: {this.IncomingPacketType:X2}\n\n{sb}");

            return null;
        }
    }
}