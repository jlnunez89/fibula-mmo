// -----------------------------------------------------------------
// <copyright file="ISocketConnection.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Abstractions
{
    /// <summary>
    /// Interface for connections.
    /// </summary>
    public interface ISocketConnection : IConnection
    {
        /// <summary>
        /// Gets the Socket IP address of this connection, if it is open.
        /// </summary>
        string SocketIp { get; }

        /// <summary>
        /// Sets up an Xtea key expected to be matched on subsequent messages.
        /// </summary>
        /// <param name="xteaKey">The XTea key to use in this connection's communications.</param>
        void SetupAuthenticationKey(uint[] xteaKey);
    }
}