// -----------------------------------------------------------------
// <copyright file="DefaultRequestHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Handlers
{
    using System.Collections.Generic;
    using System.Text;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Special kind of handler that is used as a fall back when no other handler is picked.
    /// </summary>
    public class DefaultRequestHandler : BaseRequestHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public DefaultRequestHandler(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="incomingPacket">The packet to handle.</param>
        /// <param name="client">A reference to the client from where this request originated from, for context.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutboundPacket> HandleRequest(IIncomingPacket incomingPacket, IClient client)
        {
            incomingPacket.ThrowIfNull(nameof(incomingPacket));
            client.ThrowIfNull(nameof(client));

            if (!(incomingPacket is IBytesInfo debugInfo))
            {
                this.Logger.Error($"Expected packet info of type {nameof(IBytesInfo)} but got {incomingPacket.GetType().Name}.");

                return null;
            }

            var sb = new StringBuilder();

            foreach (var b in debugInfo.Bytes)
            {
                sb.AppendFormat("{0:x2} ", b);
            }

            this.Logger.Information($"Default handler drained packet with content:\n\n{sb}");

            return null;
        }
    }
}
