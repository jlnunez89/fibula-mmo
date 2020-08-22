// -----------------------------------------------------------------
// <copyright file="IEvent.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Scheduling.Contracts.Abstractions
{
    using System;
    using Fibula.Scheduling.Contracts.Delegates;
    using Fibula.Scheduling.Contracts.Enumerations;

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
        /// Delegate fired when this event is delayed.
        /// </summary>
        event EventDelayedDelegate Delayed;

        /// <summary>
        /// Fired when this even is processed to completion (after no more repeats).
        /// </summary>
        event EventCompletedDelegate Completed;

        /// <summary>
        /// Gets a unique identifier for this event.
        /// </summary>
        string EventId { get; }

        /// <summary>
        /// Gets a string representing this event's type.
        /// </summary>
        string EventType { get; }

        /// <summary>
        /// Gets the id of the requestor of this event, if available.
        /// </summary>
        uint RequestorId { get; }

        /// <summary>
        /// Gets a value indicating whether the event can be cancelled.
        /// </summary>
        bool CanBeCancelled { get; }

        /// <summary>
        /// Gets a value indicating whether to exclude this event from telemetry logging.
        /// </summary>
        bool ExcludeFromTelemetry { get; }

        /// <summary>
        /// Gets the time after which this event should be repeated.
        /// The event is not repeated if the value is not positive.
        /// </summary>
        TimeSpan RepeatAfter { get; }

        /// <summary>
        /// Gets the event's state.
        /// </summary>
        EventState State { get; }

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

        /// <summary>
        /// Attempts to delay this event.
        /// </summary>
        /// <param name="byTime">The time by which to delay the event.</param>
        /// <returns>True if the event is successfully delayed, false otherwise.</returns>
        bool Delay(TimeSpan byTime);
    }
}
