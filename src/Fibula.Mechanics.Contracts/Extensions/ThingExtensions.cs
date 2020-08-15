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

    /// <summary>
    /// Static class with helper methods for things.
    /// </summary>
    public static class ThingExtensions
    {
        /// <summary>
        /// Calculates the remaining <see cref="TimeSpan"/> until the thing's condition is over.
        /// </summary>
        /// <param name="thing">The thing to check the conditions on.</param>
        /// <param name="type">The type of exhaustion.</param>
        /// <param name="currentTime">The current time to calculate from.</param>
        /// <returns>The <see cref="TimeSpan"/> result.</returns>
        public static TimeSpan RemainingCooldownTime(this IThing thing, ConditionType type, DateTimeOffset currentTime)
        {
            thing.ThrowIfNull(nameof(thing));

            if (!thing.Conditions.TryGetValue(type, out ICondition condition))
            {
                return TimeSpan.Zero;
            }

            var timeLeft = condition.EndTime - currentTime;

            if (timeLeft < TimeSpan.Zero)
            {
                thing.Conditions.Remove(type);

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

            if (!thing.Conditions.ContainsKey(condition.Type))
            {
                thing.Conditions[condition.Type] = condition;

                return true;
            }

            if (thing.Conditions[condition.Type].EndTime < condition.EndTime)
            {
                thing.Conditions[condition.Type].EndTime = condition.EndTime;
            }

            return false;
        }
    }
}
