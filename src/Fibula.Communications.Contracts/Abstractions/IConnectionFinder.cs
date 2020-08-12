// -----------------------------------------------------------------
// <copyright file="IConnectionFinder.cs" company="2Dudes">
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
    using System.Collections.Generic;

    /// <summary>
    /// [Deprecated] Move <see cref="IConnection"/> reference to <see cref="IPlayer"/>.
    /// Interface that represents a connection finder.
    /// </summary>
    public interface IConnectionFinder
    {
        /// <summary>
        /// Looks for a single connection with the associated player id.
        /// </summary>
        /// <param name="playerId">The player id for which to look a connection for.</param>
        /// <returns>The connection instance, if found, and null otherwise.</returns>
        IConnection FindByPlayerId(uint playerId);

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