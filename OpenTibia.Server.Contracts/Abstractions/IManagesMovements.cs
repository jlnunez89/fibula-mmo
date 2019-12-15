// -----------------------------------------------------------------
// <copyright file="IManagesMovements.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Interface for an entity that accepts, creates or otherwise manages movement events.
    /// </summary>
    public interface IManagesMovements
    {
        /// <summary>
        /// Gets any pending movement events that are due given the current time.
        /// </summary>
        /// <param name="currentTime">The current time as reference.</param>
        /// <returns>A collection of events with requested time and delay.</returns>
        IEnumerable<(IEvent evt, DateTimeOffset requestedTime, TimeSpan delay)> GetMovements(DateTimeOffset currentTime);

        /// <summary>
        /// Adds a movment event to this entity to track.
        /// </summary>
        /// <param name="evt">The event to add.</param>
        /// <param name="currentTime">The current time as reference.</param>
        /// <param name="intendedDelay">The delay intended for this movement to happen.</param>
        void AddMovementEvent(IEvent evt, DateTimeOffset currentTime, TimeSpan intendedDelay);

        /// <summary>
        /// Clears all the movement events from this entity.
        /// </summary>
        void ClearAllMovementEvents();
    }
}
