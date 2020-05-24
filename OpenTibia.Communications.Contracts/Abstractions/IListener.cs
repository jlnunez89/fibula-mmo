// -----------------------------------------------------------------
// <copyright file="IListener.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts.Abstractions
{
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Common interface of all open tibia listeners.
    /// </summary>
    public interface IListener : IHostedService
    {
        /// <summary>
        /// Processes an incomming message from the connection.
        /// </summary>
        /// <param name="connection">The connection where the message is being read from.</param>
        /// <param name="inboundMessage">The message to process.</param>
        void ProcessMessage(IConnection connection, INetworkMessage inboundMessage);

        /// <summary>
        /// Runs after processing a message from the connection.
        /// </summary>
        /// <param name="connection">The connection where the message is from.</param>
        void PostProcessMessage(IConnection connection);
    }
}