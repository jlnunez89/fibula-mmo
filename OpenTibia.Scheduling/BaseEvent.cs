// <copyright file="BaseEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Scheduling
{
    using System.Collections.Generic;
    using OpenTibia.Server.Interfaces;
    using Priority_Queue;

    /// <summary>
    /// Abstract class that represents the base event for scheduling.
    /// </summary>
    public abstract class BaseEvent : FastPriorityQueueNode, IEvent
    {
        /// <inheritdoc/>
        public abstract EvaluationTime EvaluateAt { get; }

        /// <inheritdoc/>
        public abstract bool CanBeExecuted { get; }

        /// <inheritdoc/>
        public abstract IEnumerable<IEventFunction> Conditions { get; }

        /// <inheritdoc/>
        public abstract IEnumerable<IEventFunction> Actions { get; }

        /// <inheritdoc/>
        public abstract IDictionary<string, IEventArgument> Arguments { get; }

        /// <inheritdoc/>
        public abstract void Execute();
    }
}