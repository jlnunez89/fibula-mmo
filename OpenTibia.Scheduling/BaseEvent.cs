// -----------------------------------------------------------------
// <copyright file="BaseEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Scheduling
{
    using System;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using Priority_Queue;

    /// <summary>
    /// Abstract class that represents the base event for scheduling.
    /// </summary>
    public abstract class BaseEvent : FastPriorityQueueNode, IEvent
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
        }

        /// <summary>
        /// Fired when this event is expedited.
        /// </summary>
        public event EventExpedited Expedited;

        /// <summary>
        /// Gets a unique identifier for this event.
        /// </summary>
        public string EventId { get; }

        /// <summary>
        /// Gets the id of the requestor of this event, if available.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the event should be repeated.
        /// </summary>
        public bool Repeat { get; set; }

        /// <summary>
        /// Gets or sets a value for how long to wait until the event should be repeated.
        /// </summary>
        public TimeSpan RepeatDelay { get; set; }

        /// <summary>
        /// Gets a value indicating whether this event has a handler hooked up for it's <see cref="Expedited"/> event.
        /// </summary>
        public bool HasExpeditionHandler => this.Expedited != null;

        /// <summary>
        /// Executes the event logic.
        /// </summary>
        /// <param name="context">The execution context.</param>
        public abstract void Execute(IEventContext context);

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
    }
}