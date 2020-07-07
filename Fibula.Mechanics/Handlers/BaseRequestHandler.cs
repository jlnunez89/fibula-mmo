// -----------------------------------------------------------------
// <copyright file="BaseRequestHandler.cs" company="2Dudes">
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
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents the base implementation for all request handlers in all protocols.
    /// </summary>
    public abstract class BaseRequestHandler : IHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        protected BaseRequestHandler(ILogger logger)
        {
            logger.ThrowIfNull(nameof(logger));

            this.Logger = logger.ForContext(this.GetType());
        }

        /// <summary>
        /// Gets the reference to the logger in use.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="incomingPacket">The packet to handle.</param>
        /// <param name="client">A reference to the client from where this request originated from, for context.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s that compose that synchronous response, if any.</returns>
        public abstract IEnumerable<IOutboundPacket> HandleRequest(IIncomingPacket incomingPacket, IClient client);
    }
}
