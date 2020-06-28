// -----------------------------------------------------------------
// <copyright file="IEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Scheduling.Contracts.Abstractions
{
    using System;
    using Fibula.Scheduling.Contracts.Delegates;

    /// <summary>
    /// Interface that represents an event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Delegate fired when this event is cancelled.
        /// </summary>
        event EventCancelledDelegate Cancelled;

        /// <summary>
        /// Delegate fired when this event is expedited.
        /// </summary>
        event EventExpeditedDelegate Expedited;

        /// <summary>
        /// Gets a unique identifier for this event.
        /// </summary>
        string EventId { get; }

        /// <summary>
        /// Gets the id of the requestor of this event, if available.
        /// </summary>
        uint RequestorId { get; }

        /// <summary>
        /// Gets a value indicating whether the event can be cancelled.
        /// </summary>
        bool CanBeCancelled { get; }

        /// <summary>
        /// Gets the time after which this event should be repeated.
        /// The event is not repeated if the value is not positive.
        /// </summary>
        TimeSpan RepeatAfter { get; }

        /// <summary>
        /// Attempts to cancel this event.
        /// </summary>
        /// <returns>True if the event is successfully cancelled, false otherwise.</returns>
        bool Cancel();

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