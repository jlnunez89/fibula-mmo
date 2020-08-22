// -----------------------------------------------------------------
// <copyright file="ExhaustionCondition.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a condition for exhaustion.
    /// </summary>
    public class ExhaustionCondition : Condition, IExhaustionCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExhaustionCondition"/> class.
        /// </summary>
        /// <param name="exhaustionType">The type of exhaustion.</param>
        /// <param name="endTime">The date and time at which the condition is set to end.</param>
        public ExhaustionCondition(ExhaustionType exhaustionType, DateTimeOffset endTime)
            : base(ConditionType.Exhausted)
        {
            this.ExhaustionTimesPerType = new Dictionary<ExhaustionType, DateTimeOffset>()
            {
                { exhaustionType, endTime },
            };

            this.ExcludeFromTelemetry = true;
        }

        /// <summary>
        /// Gets the current types that this exhaustion covers.
        /// </summary>
        public IDictionary<ExhaustionType, DateTimeOffset> ExhaustionTimesPerType { get; }

        /// <summary>
        /// Aggregates this condition into another of the same type.
        /// </summary>
        /// <param name="conditionOfSameType">The condition to aggregate into.</param>
        /// <returns>True if the conditions were aggregated (changed), and false if nothing was done.</returns>
        public override bool Aggregate(ICondition conditionOfSameType)
        {
            conditionOfSameType.ThrowIfNull(nameof(conditionOfSameType));

            if (!(conditionOfSameType is ExhaustionCondition otherExhaustionCondition))
            {
                return false;
            }

            var changed = false;

            foreach (var (exhaustionType, newEndTime) in otherExhaustionCondition.ExhaustionTimesPerType)
            {
                // Only carry over if out end times are greater.
                if (this.ExhaustionTimesPerType.TryGetValue(exhaustionType, out DateTimeOffset currentEndTime) && newEndTime < currentEndTime)
                {
                    continue;
                }

                this.ExhaustionTimesPerType[exhaustionType] = newEndTime;

                changed = true;
            }

            return changed;
        }

        /// <summary>
        /// Executes the condition's logic.
        /// </summary>
        /// <param name="context">The execution context for this condition.</param>
        protected override void Execute(IConditionContext context)
        {
            var currentTimeSnapshot = context.CurrentTime;

            // Clear out any exhaustion that has passed.
            foreach (var exhaustionType in this.ExhaustionTimesPerType.Keys)
            {
                if (this.ExhaustionTimesPerType[exhaustionType] > currentTimeSnapshot)
                {
                    continue;
                }

                this.ExhaustionTimesPerType.Remove(exhaustionType);
            }

            if (this.ExhaustionTimesPerType.Count == 0)
            {
                return;
            }

            var nextExpiry = this.ExhaustionTimesPerType.Min(kvp => kvp.Value - currentTimeSnapshot);

            this.RepeatAfter = nextExpiry < TimeSpan.Zero ? TimeSpan.Zero : nextExpiry;
        }
    }
}
