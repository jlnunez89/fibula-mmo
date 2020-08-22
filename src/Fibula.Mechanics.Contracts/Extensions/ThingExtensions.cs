// -----------------------------------------------------------------
// <copyright file="ThingExtensions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Extensions
{
    using System;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Static class with helper methods for things.
    /// </summary>
    public static class ThingExtensions
    {
        /// <summary>
        /// Calculates the remaining <see cref="TimeSpan"/> until the thing's exhaustion is over.
        /// </summary>
        /// <param name="thing">The thing to check the conditions on.</param>
        /// <param name="exhaustionType">The type of condition.</param>
        /// <param name="currentTime">The current time to calculate from.</param>
        /// <returns>The <see cref="TimeSpan"/> result.</returns>
        public static TimeSpan RemainingExhaustionTime(this IThing thing, ExhaustionType exhaustionType, DateTimeOffset currentTime)
        {
            thing.ThrowIfNull(nameof(thing));

            if (!thing.IsExhausted(exhaustionType))
            {
                return TimeSpan.Zero;
            }

            var exhaustionCondition = thing.TrackedEvents[ConditionType.Exhausted.ToString()] as IExhaustionCondition;

            if (!exhaustionCondition.ExhaustionTimesPerType.TryGetValue(exhaustionType, out DateTimeOffset exhaustionEndTime))
            {
                return TimeSpan.Zero;
            }

            var timeLeft = exhaustionEndTime - currentTime;

            return timeLeft < TimeSpan.Zero ? TimeSpan.Zero : timeLeft;
        }

        /// <summary>
        /// Checks if the thing has the given condition.
        /// </summary>
        /// <param name="thing">The thing to check the conditions on.</param>
        /// <param name="conditionType">The type of condition.</param>
        /// <returns>True if the thing has such condition, false otherwise.</returns>
        public static bool HasCondition(this IThing thing, ConditionType conditionType)
        {
            thing.ThrowIfNull(nameof(thing));
            conditionType.ThrowIfNull(nameof(conditionType));

            return thing.TrackedEvents.ContainsKey(conditionType.ToString());
        }

        /// <summary>
        /// Checks if the thing is exhausted.
        /// </summary>
        /// <param name="thing">The thing to check the exhaustion condition on.</param>
        /// <param name="type">The type of exhaustion to check for.</param>
        /// <returns>True if the thing has such condition, false otherwise.</returns>
        public static bool IsExhausted(this IThing thing, ExhaustionType type)
        {
            thing.ThrowIfNull(nameof(thing));
            type.ThrowIfNull(nameof(type));

            return thing.TrackedEvents.TryGetValue(ConditionType.Exhausted.ToString(), out IEvent conditionEvent) &&
                    conditionEvent is IExhaustionCondition exhaustionCondition &&
                    exhaustionCondition.ExhaustionTimesPerType.ContainsKey(type);
        }
    }
}
