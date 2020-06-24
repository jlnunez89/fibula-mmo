// -----------------------------------------------------------------
// <copyright file="IConnectionManager.cs" company="2Dudes">
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
    /// [Possibly Deprecated] Move <see cref="IConnection"/> reference to <see cref="IPlayer"/>.
    /// Interface that represents a connection manager.
    /// </summary>
    public interface IConnectionManager : IConnectionFinder
    {
        /// <summary>
        /// Registers a new connection to the manager.
        /// </summary>
        /// <param name="connection">The connection to register.</param>
        /// <param name="playerId">The id of the player that this connection is tied to.</param>
        void Register(IConnection connection, uint playerId);

        /// <summary>
        /// Unregisters a connection from the manager.
        /// </summary>
        /// <param name="connection">The connection to unregister.</param>
        void Unregister(IConnection connection);
    }
}