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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface that represents a connection manager.
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>
        /// Registers a new connection to the manager.
        /// </summary>
        /// <param name="connection">The connection to register.</param>
        void Register(IConnection connection);

        /// <summary>
        /// Unregisters a connection from the manager.
        /// </summary>
        /// <param name="connection">The connection to unregister.</param>
        void Unregister(IConnection connection);

        /// <summary>
        /// Looks for a single connection with the associated player id.
        /// </summary>
        /// <param name="playerId">The player id for which to look a connection for.</param>
        /// <returns>The connection instance, if found, and null otherwise.</returns>
        IConnection FindByPlayerId(Guid playerId);

        /// <summary>
        /// Gets all active connections known to this manager.
        /// </summary>
        /// <returns>A collection of connection instances.</returns>
        IEnumerable<IConnection> GetAllActive();

        /// <summary>
        /// Gets all orphaned connections known to this manager.
        /// </summary>
        /// <returns>A collection of connection instances.</returns>
        IEnumerable<IConnection> GetAllOrphaned();
    }
}