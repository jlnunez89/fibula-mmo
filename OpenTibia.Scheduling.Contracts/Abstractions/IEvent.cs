// -----------------------------------------------------------------
// <copyright file="IEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Scheduling.Contracts.Abstractions
{
    using System;

    /// <summary>
    /// Interface that represents an event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Fired when this event is expedited.
        /// </summary>
        event EventExpedited Expedited;

        /// <summary>
        /// Gets a unique identifier for this event.
        /// </summary>
        string EventId { get; }

        /// <summary>
        /// Gets the id of the requestor of this event, if available.
        /// </summary>
        uint RequestorId { get; }

        /// <summary>
        /// Gets a value indicating whether the event should be repeated.
        /// </summary>
        bool Repeat { get; }

        /// <summary>
        /// Gets a value for how long to wait until the event should be repeated.
        /// </summary>
        TimeSpan RepeatDelay { get; }

        /// <summary>
        /// Executes the event logic.
        /// </summary>
        /// <param name="context">The execution context.</param>
        void Execute(IEventContext context);

        /// <summary>
        /// Attempts to expedite this event, requesting it to be fired immediately.
        /// </summary>
        /// <returns>True if the event is successfully expedited, false otherwise.</returns>
        bool Expedite();
    }
}