// <copyright file="BaseEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Scheduling
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Scheduling.Contracts;
    using Priority_Queue;

    /// <summary>
    /// Abstract class that represents the base event for scheduling.
    /// </summary>
    public abstract class BaseEvent : FastPriorityQueueNode, IEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEvent"/> class.
        /// </summary>
        public BaseEvent()
        {
            this.EventId = Guid.NewGuid().ToString("N");
            this.RequestorId = 0;

            this.Conditions = new List<IEventCondition>();
            this.ActionsOnPass = new List<IEventAction>();
            this.ActionsOnFail = new List<IEventAction>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEvent"/> class.
        /// </summary>
        /// <param name="requestorId">Optional. The id of the creature or entity requesting the event.</param>
        public BaseEvent(uint requestorId = 0)
            : this()
        {
            this.RequestorId = requestorId;
        }

        /// <inheritdoc/>
        public string EventId { get; }

        /// <inheritdoc/>
        public uint RequestorId { get; }

        /// <inheritdoc/>
        public string ErrorMessage { get; protected set; }

        /// <inheritdoc/>
        public abstract EvaluationTime EvaluateAt { get; }

        /// <inheritdoc/>
        public abstract bool CanBeExecuted { get; }

        /// <inheritdoc/>
        public IList<IEventCondition> Conditions { get; }

        /// <inheritdoc/>
        public IList<IEventAction> ActionsOnPass { get; }

        /// <inheritdoc/>
        public IList<IEventAction> ActionsOnFail { get; }

        /// <inheritdoc/>
        public abstract void Process();
    }
}