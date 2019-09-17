// -----------------------------------------------------------------
// <copyright file="BaseProtocol.cs" company="2Dudes">
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
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a base for all protocols.
    /// </summary>
    public abstract class BaseProtocol : IProtocol
    {
        /// <summary>
        /// A value indicating whether the protocol should keep the connection open after recieving a packet.
        /// </summary>
        private readonly bool keepConnectionOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseProtocol"/> class.
        /// </summary>
        /// <param name="handlerSelector">A reference to the handler selector to use.</param>
        /// <param name="keepConnectionOpen">Optional. A value indicating whether to maintain the connection open after processing a message in the connection.</param>
        protected BaseProtocol(IHandlerSelector handlerSelector, bool keepConnectionOpen = true)
        {
            handlerSelector.ThrowIfNull(nameof(handlerSelector));

            this.HandlerSelector = handlerSelector;
            this.keepConnectionOpen = keepConnectionOpen;
        }

        /// <summary>
        /// Gets an instance of the handler selector.
        /// </summary>
        protected IHandlerSelector HandlerSelector { get; private set; }

        /// <summary>
        /// Processes an incomming message from the connection.
        /// </summary>
        /// <param name="connection">The connection where the message is being read from.</param>
        /// <param name="inboundMessage">The message to process.</param>
        public abstract void ProcessMessage(IConnection connection, INetworkMessage inboundMessage);

        /// <summary>
        /// Runs after processing a message from the connection.
        /// </summary>
        /// <param name="connection">The connection where the message is from.</param>
        public virtual void PostProcessMessage(IConnection connection)
        {
            if (!this.keepConnectionOpen)
            {
                connection.Close();
                return;
            }

            connection.InboundMessage.Reset();
            connection.BeginStreamRead();
        }
    }
}
