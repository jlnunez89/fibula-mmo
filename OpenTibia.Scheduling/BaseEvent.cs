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
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Scheduling.Contracts.Enumerations;
    using Priority_Queue;

    /// <summary>
    /// Abstract class that represents the base event for scheduling.
    /// </summary>
    public abstract class BaseEvent : FastPriorityQueueNode, IEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEvent"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="evaluationTime">Optional. The time at which the event's conditions should be evaluated. Default is <see cref="EvaluationTime.OnExecute"/>.</param>
        public BaseEvent(ILogger logger, EvaluationTime evaluationTime = EvaluationTime.OnExecute)
        {
            logger.ThrowIfNull(nameof(logger));

            this.EventId = Guid.NewGuid().ToString("N");
            this.RequestorId = 0;
            this.EvaluateAt = evaluationTime;
            this.Logger = logger;

            this.Conditions = new List<IEventCondition>();
            this.ActionsOnPass = new List<IEventAction>();
            this.ActionsOnFail = new List<IEventAction>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEvent"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="requestorId">Optional. The id of the creature or entity requesting the event. Default is 0.</param>
        /// <param name="evaluationTime">Optional. The time at which the event's conditions should be evaluated. Default is <see cref="EvaluationTime.OnExecute"/>.</param>
        public BaseEvent(ILogger logger, uint requestorId = 0, EvaluationTime evaluationTime = EvaluationTime.OnExecute)
            : this(logger, evaluationTime)
        {
            this.RequestorId = requestorId;
        }

        /// <summary>
        /// Gets a unique identifier for this event.
        /// </summary>
        public string EventId { get; }

        /// <summary>
        /// Gets the id of the requestor of this event, if available.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets a reference to the logger instance.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets or sets the error message that should be bubbled back to the player if the event cannot be executed.
        /// </summary>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// Gets a <see cref="EvaluationTime"/> value indicating when this event should be evaluated.
        /// </summary>
        public EvaluationTime EvaluateAt { get; }

        /// <summary>
        /// Gets a value indicating whether the event can be executed.
        /// </summary>
        public bool CanBeExecuted
        {
            get
            {
                var allPassed = true;

                foreach (var policy in this.Conditions)
                {
                    allPassed &= policy.Evaluate();

                    if (!allPassed)
                    {
                        this.Logger.LogDebug($"Failed event condition {policy.GetType().Name}.");
                        this.ErrorMessage = policy.ErrorMessage;
                        break;
                    }
                }

                return allPassed;
            }
        }

        /// <inheritdoc/>
        public IList<IEventCondition> Conditions { get; }

        /// <inheritdoc/>
        public IList<IEventAction> ActionsOnPass { get; }

        /// <inheritdoc/>
        public IList<IEventAction> ActionsOnFail { get; }

        /// <summary>
        /// Executes the event. Performs the <see cref="ActionsOnPass"/> on the <see cref="ActionsOnFail"/> depending if the conditions were met.
        /// </summary>
        public void Process()
        {
            Stopwatch sw = new Stopwatch();

            if (this.EvaluateAt == EvaluationTime.OnSchedule || this.CanBeExecuted)
            {
                for (int i = 1; i <= this.ActionsOnPass.Count; i++)
                {
                    sw.Restart();

                    this.ActionsOnPass[i].Execute();

                    sw.Stop();

                    this.Logger.LogTrace($"Executed ({i++} of {this.ActionsOnPass.Count}) on pass... done in {sw.Elapsed}.");
                }

                return;
            }

            for (int i = 1; i <= this.ActionsOnFail.Count; i++)
            {
                sw.Restart();

                this.ActionsOnFail[i].Execute();

                sw.Stop();

                this.Logger.LogTrace($"Executed ({i++} of {this.ActionsOnFail.Count}) actions on fail... done in {sw.Elapsed}.");
            }
        }
    }
}