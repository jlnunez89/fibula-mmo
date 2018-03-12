// <copyright file="IEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface that represents an event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Gets a <see cref="EvaluationTime"/> value indicating when this event should be evaluated.
        /// </summary>
        EvaluationTime EvaluateAt { get; }

        /// <summary>
        /// Gets a value indicating whether the event can be executed.
        /// </summary>
        bool CanBeExecuted { get; }

        /// <summary>
        /// Gets the collection of conditional <see cref="IEventFunction"/> that the event must pass on evaluation.
        /// </summary>
        IEnumerable<IEventFunction> Conditions { get; }

        /// <summary>
        /// Gets the collection of <see cref="IEventFunction"/> that will be executed as part of this event.
        /// </summary>
        IEnumerable<IEventFunction> Actions { get; }

        /// <summary>
        /// Gets a dictionary of <see cref="IEventArgument"/> that is available to all conditions and actions.
        /// </summary>
        IDictionary<string, IEventArgument> Arguments { get; }

        /// <summary>
        /// Executes the event.
        /// </summary>
        void Execute();
    }
}