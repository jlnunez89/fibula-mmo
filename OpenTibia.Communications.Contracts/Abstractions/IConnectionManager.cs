// -----------------------------------------------------------------
// <copyright file="IConnectionManager.cs" company="2Dudes">
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
    /// <summary>
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