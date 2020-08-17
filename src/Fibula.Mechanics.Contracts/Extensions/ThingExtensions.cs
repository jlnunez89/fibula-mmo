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
        /// Calculates the remaining <see cref="TimeSpan"/> until the thing's condition is over.
        /// </summary>
        /// <param name="thing">The thing to check the conditions on.</param>
        /// <param name="type">The type of condition.</param>
        /// <param name="currentTime">The current time to calculate from.</param>
        /// <returns>The <see cref="TimeSpan"/> result.</returns>
        public static TimeSpan RemainingCooldownTime(this IThing thing, ConditionType type, DateTimeOffset currentTime)
        {
            thing.ThrowIfNull(nameof(thing));

            if (!thing.TrackedEvents.TryGetValue(type.ToString(), out IEvent conditionEvent) || !(conditionEvent is ICondition condition))
            {
                return TimeSpan.Zero;
            }

            var timeLeft = condition.EndTime - currentTime;

            if (timeLeft < TimeSpan.Zero)
            {
                return TimeSpan.Zero;
            }

            return timeLeft;
        }

        /// <summary>
        /// Adds or extends a condition to the afflicted thing.
        /// </summary>
        /// <param name="thing">The thing to check the conditions on.</param>
        /// <param name="condition">The condition to add or extend.</param>
        /// <returns>True if the condition was added, false otherwise.</returns>
        public static bool AddOrExtendCondition(this IThing thing, ICondition condition)
        {
            thing.ThrowIfNull(nameof(thing));
            condition.ThrowIfNull(nameof(condition));

            if (!thing.TrackedEvents.TryGetValue(condition.GetType().Name, out IEvent conditionEvent) || !(conditionEvent is ICondition existingCondition))
            {
                thing.StartTrackingEvent(condition);

                return true;
            }

            if (existingCondition.EndTime < condition.EndTime)
            {
                existingCondition.EndTime = condition.EndTime;
            }

            return false;
        }

        /// <summary>
        /// Checks if the thing has the given condition.
        /// </summary>
        /// <param name="thing">The thing to check the conditions on.</param>
        /// <param name="type">The type of condition.</param>
        /// <returns>True if the thing has such condition, false otherwise.</returns>
        public static bool HasCondition(this IThing thing, Type type)
        {
            thing.ThrowIfNull(nameof(thing));
            type.ThrowIfNull(nameof(type));

            return thing.TrackedEvents.ContainsKey(type.Name);
        }
    }
}
