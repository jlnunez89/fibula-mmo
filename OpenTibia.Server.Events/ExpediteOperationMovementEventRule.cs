// -----------------------------------------------------------------
// <copyright file="ExpediteOperationMovementEventRule.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    public class ExpediteOperationMovementEventRule : IThingMovementEventRule
    {
        public ExpediteOperationMovementEventRule(
            ILogger logger,
            IOperation operationToExpedite,
            IEnumerable<Func<IEventRuleContext, bool>> conditionsForExpedition = null,
            int totalExecutionCount = IEventRule.UnlimitedExecutions)
        {
            logger.ThrowIfNull(nameof(logger));
            operationToExpedite.ThrowIfNull(nameof(operationToExpedite));

            this.Logger = logger.ForContext(this.GetType());

            this.Id = Guid.NewGuid().ToString("N");
            this.TargetOperation = operationToExpedite;
            this.Conditions = conditionsForExpedition;

            this.RemainingExecutionCount = totalExecutionCount;
        }

        public string Id { get; }

        /// <summary>
        /// Gets the conditions for the operation expedition to happen.
        /// </summary>
        public IEnumerable<Func<IEventRuleContext, bool>> Conditions { get; }

        public IOperation TargetOperation { get; }

        public EventRuleType Type => EventRuleType.Movement;

        public ILogger Logger { get; }

        public int RemainingExecutionCount { get; private set; }

        /// <summary>
        /// Checks whether this event rule can be executed.
        /// </summary>
        /// <param name="context">The execution context of this rule.</param>
        /// <returns>True if the rule can be executed, false otherwise.</returns>
        public bool CanBeExecuted(IEventRuleContext context)
        {
            context.ThrowIfNull(nameof(context));

            return this.Conditions == null || this.Conditions.All(condition => condition(context));
        }

        /// <summary>
        /// Executes this event rule.
        /// </summary>
        /// <param name="context">The execution context of this rule.</param>
        public void Execute(IEventRuleContext context)
        {
            context.ThrowIfNull(nameof(context));

            if (!this.TargetOperation.Expedite())
            {
                context.Scheduler.ScheduleEvent(this.TargetOperation);
            }

            this.Logger.Verbose($"Expedited operation {this.TargetOperation.EventId}.");

            if (this.RemainingExecutionCount != IEventRule.UnlimitedExecutions && this.RemainingExecutionCount > 0)
            {
                this.RemainingExecutionCount--;
            }
        }
    }
}
