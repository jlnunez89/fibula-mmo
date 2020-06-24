// -----------------------------------------------------------------
// <copyright file="ISocketConnectionFactory.cs" company="2Dudes">
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
    using System.Net.Sockets;

    /// <summary>
    /// Interface that represents a connection factory.
    /// </summary>
    public interface ISocketConnectionFactory
    {
        /// <summary>
        /// Creates a new <see cref="ISocketConnection"/> for the given socket.
        /// </summary>
        /// <param name="socket">The socket of the connection.</param>
        /// <returns>A new instance of a <see cref="ISocketConnection"/>.</returns>
        ISocketConnection Create(Socket socket);
    }
}