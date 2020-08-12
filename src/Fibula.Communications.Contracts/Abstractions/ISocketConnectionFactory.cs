// -----------------------------------------------------------------
// <copyright file="ISocketConnectionFactory.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
