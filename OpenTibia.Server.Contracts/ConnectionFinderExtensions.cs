// -----------------------------------------------------------------
// <copyright file="ConnectionFinderExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Helper class that provides extensions for the <see cref="IConnectionFinder"/> implementations.
    /// </summary>
    public static class ConnectionFinderExtensions
    {
        /// <summary>
        /// Gets the connections of any players that can see the given locations.
        /// </summary>
        /// <param name="connectionFinder">The reference to the connection finder.</param>
        /// <param name="creatureFinder">The reference to the creature finder.</param>
        /// <param name="locations">The locations to check if players can see.</param>
        /// <returns>A collection of connections.</returns>
        public static IEnumerable<IConnection> PlayersThatCanSee(this IConnectionFinder connectionFinder, ICreatureFinder creatureFinder, params Location[] locations)
        {
            connectionFinder.ThrowIfNull(nameof(connectionFinder));
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            var activeConnections = connectionFinder.GetAllActive();

            foreach (var connection in activeConnections)
            {
                var player = creatureFinder.FindCreatureById(connection.PlayerId);

                if (player == null || !locations.Any(loc => player.CanSee(loc)))
                {
                    continue;
                }

                yield return connection;
            }
        }
    }
}
