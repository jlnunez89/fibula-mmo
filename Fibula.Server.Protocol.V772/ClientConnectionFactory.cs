﻿// -----------------------------------------------------------------
// <copyright file="ClientConnectionFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Protocol.V772
{
    using System.Net.Sockets;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that implements a <see cref="ISocketConnectionFactory"/> which creates new connections that
    /// target a given protocol.
    /// </summary>
    /// <typeparam name="TProtocol">The type of protocol to use.</typeparam>
    public class ClientConnectionFactory<TProtocol> : ISocketConnectionFactory
        where TProtocol : IProtocol
    {
        /// <summary>
        /// Stores the logger instance.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Stores the protocol to target.
        /// </summary>
        private readonly TProtocol protocol;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnectionFactory{TProtocol}"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="protocol">A reference to the protocol to target.</param>
        public ClientConnectionFactory(ILogger logger, TProtocol protocol)
        {
            logger.ThrowIfNull(nameof(logger));
            protocol.ThrowIfNull(nameof(protocol));

            this.logger = logger.ForContext<ClientConnectionFactory<TProtocol>>();
            this.protocol = protocol;
        }

        /// <summary>
        /// Creates a new <see cref="ISocketConnection"/> for the given socket.
        /// </summary>
        /// <param name="socket">The socket of the connection.</param>
        /// <returns>A new instance of a <see cref="ISocketConnection"/>.</returns>
        public ISocketConnection Create(Socket socket)
        {
            return new StandardTcpClientConnection(this.logger, socket, this.protocol);
        }
    }
}
