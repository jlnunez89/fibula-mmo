// -----------------------------------------------------------------
// <copyright file="BaseEvent.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Scheduling
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Fibula.Scheduling.Contracts.Delegates;
    using Fibula.Scheduling.Contracts.Enumerations;
    using Priority_Queue;

    /// <summary>
    /// Abstract class that represents the base event for scheduling.
    /// </summary>
    public abstract class BaseEvent : StablePriorityQueueNode, IEvent, IEquatable<BaseEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEvent"/> class.
        /// </summary>
        /// <param name="requestorId">Optional. The id of the creature or entity requesting the event. Default is 0.</param>
        public BaseEvent(uint requestorId = 0)
        {
            this.EventId = Guid.NewGuid().ToString("N");
            this.RequestorId = 0;

            this.RequestorId = requestorId;

            this.RepeatAfter = TimeSpan.MinValue;

            this.State = EventState.Created;
        }

        /// <summary>
        /// Fired when this event is cancelled.
        /// </summary>
        public event EventCancelledDelegate Cancelled;

        /// <summary>
        /// Fired when this event is expedited.
        /// </summary>
        public event EventExpeditedDelegate Expedited;

        /// <summary>
        /// Fired when this event is delayed.
        /// </summary>
        public event EventDelayedDelegate Delayed;

        /// <summary>
        /// Fired when this even is processed to completion (after no more repeats).
        /// </summary>
        public event EventCompletedDelegate Completed;

        /// <summary>
        /// Gets a unique identifier for this event.
        /// </summary>
        public string EventId { get; }

        /// <summary>
        /// Gets a string representing this event's type.
        /// </summary>
        public abstract string EventType { get; }

        /// <summary>
        /// Gets the id of the requestor of this event, if available.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets or sets the time after which this event should be repeated.
        /// The event is not repeated if the value is not positive.
        /// </summary>
        public TimeSpan RepeatAfter { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this event has a handler hooked up for it's <see cref="Cancelled"/> event.
        /// </summary>
        public bool HasCancellationHandler => this.Cancelled != null;

        /// <summary>
        /// Gets a value indicating whether this event has a handler hooked up for it's <see cref="Expedited"/> event.
        /// </summary>
        public bool HasExpeditionHandler => this.Expedited != null;

        /// <summary>
        /// Gets a value indicating whether this event has a handler hooked up for it's <see cref="Delayed"/> event.
        /// </summary>
        public bool HasDelayHandler => this.Delayed != null;

        /// <summary>
        /// Gets or sets a value indicating whether the event can be cancelled.
        /// </summary>
        public abstract bool CanBeCancelled { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether to exclude this event from telemetry logging.
        /// </summary>
        public bool ExcludeFromTelemetry { get; protected set; }

        /// <summary>
        /// Gets or sets the event's state.
        /// </summary>
        public EventState State { get; set; }

        /// <summary>
        /// Executes the event logic.
        /// </summary>
        /// <param name="context">The execution context.</param>
        public abstract void Execute(IEventContext context);

        /// <summary>
        /// Attempts to cancel this event.
        /// </summary>
        /// <returns>True if the event is successfully cancelled, false otherwise.</returns>
        public bool Cancel()
        {
            if (this.Cancelled == null)
            {
                return false;
            }

            return this.Cancelled.Invoke(this);
        }

        /// <summary>
        /// Attempts to expedite this event, in other words, requesting it to be fired immediately.
        /// </summary>
        /// <returns>True if the event is successfully expedited, false otherwise.</returns>
        public bool Expedite()
        {
            if (this.Expedited == null)
            {
                return false;
            }

            return this.Expedited.Invoke(this);
        }

        /// <summary>
        /// Attempts to delay this event.
        /// </summary>
        /// <param name="byTime">The time by which to delay the event.</param>
        /// <returns>True if the event is successfully delayed, false otherwise.</returns>
        public bool Delay(TimeSpan byTime)
        {
            if (this.Delayed == null)
            {
                return false;
            }

            return this.Delayed.Invoke(this, byTime);
        }

        /// <summary>
        /// Marks this event as completed, notifying subscribers of it's <see cref="Completed"/> event.
        /// </summary>
        /// <param name="asCancelled">Optional. A value indicating whether the event was cancelled.</param>
        public void Complete(bool asCancelled = false)
        {
            this.State = asCancelled ? EventState.Cancelled : EventState.Completed;

            this.Completed?.Invoke(this);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">The other object to compare against.</param>
        /// <returns>True if the current object is equal to the other parameter, false otherwise.</returns>
        public bool Equals([AllowNull] BaseEvent other)
        {
            return this.EventId == other?.EventId;
        }
    }
}
