// -----------------------------------------------------------------
// <copyright file="ConnectionManager.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a standard manager for connections which is thread safe.
    /// </summary>
    public class ConnectionManager : IConnectionManager
    {
        /// <summary>
        /// Gets the <see cref="ConcurrentDictionary{TKey,TValue}"/> of all <see cref="IConnection"/>s, in which the Key is the <see cref="IConnection.PlayerId"/>.
        /// </summary>
        private readonly ConcurrentDictionary<uint, IConnection> connectionsMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionManager"/> class.
        /// </summary>
        public ConnectionManager()
        {
            this.connectionsMap = new ConcurrentDictionary<uint, IConnection>();
        }

        /// <summary>
        /// Registers a new connection to the manager.
        /// </summary>
        /// <param name="connection">The connection to register.</param>
        /// <param name="playerId">The id of the player that this connection is tied to.</param>
        public void Register(IConnection connection, uint playerId)
        {
            connection.ThrowIfNull(nameof(connection));
            playerId.ThrowIfDefaultValue(nameof(playerId));

            // Associate the connection to the new player before adding it.
            connection.AssociateToPlayer(playerId);

            this.connectionsMap.TryAdd(connection.PlayerId, connection);
        }

        /// <summary>
        /// Unregisters a connection from the manager.
        /// </summary>
        /// <param name="connection">The connection to unregister.</param>
        public void Unregister(IConnection connection)
        {
            connection.ThrowIfNull(nameof(connection));

            this.connectionsMap.TryRemove(connection.PlayerId, out _);
        }

        /// <summary>
        /// Looks for a single connection with the associated player id.
        /// </summary>
        /// <param name="playerId">The player id for which to look a connection for.</param>
        /// <returns>The connection instance, if found, and null otherwise.</returns>
        public IConnection FindByPlayerId(uint playerId)
        {
            if (this.connectionsMap.TryGetValue(playerId, out IConnection connection))
            {
                return connection;
            }

            return null;
        }

        /// <summary>
        /// Gets all active connections known to this manager.
        /// </summary>
        /// <returns>A collection of connection instances.</returns>
        public IEnumerable<IConnection> GetAllActive()
        {
            // ConcurrentDictionary.Values produces a moment-in-time snapshot, so no need to do ToList().
            return this.connectionsMap.Values.Where(c => !c.IsOrphaned);
        }

        /// <summary>
        /// Gets all orphaned connections known to this manager.
        /// </summary>
        /// <returns>A collection of connection instances.</returns>
        public IEnumerable<IConnection> GetAllOrphaned()
        {
            // ConcurrentDictionary.Values produces a moment-in-time snapshot, so no need to do ToList().
            return this.connectionsMap.Values.Where(c => c.IsOrphaned);
        }
    }
}
