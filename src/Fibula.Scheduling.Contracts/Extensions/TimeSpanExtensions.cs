// -----------------------------------------------------------------
// <copyright file="TimeSpanExtensions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Scheduling.Contracts.Extensions
{
    using System;

    /// <summary>
    /// Static class that contains helper methods for <see cref="TimeSpan"/>s.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Rounds the given time span by another.
        /// </summary>
        /// <param name="timeSpan">The time span to round.</param>
        /// <param name="roundBy">The time span to round by.</param>
        /// <returns>The rounded time.</returns>
        public static TimeSpan Round(this TimeSpan timeSpan, TimeSpan roundBy)
        {
            if (roundBy == TimeSpan.Zero)
            {
                return timeSpan;
            }

            var rndTicks = roundBy.Ticks;

            var ansTicks = timeSpan.Ticks + (Math.Sign(timeSpan.Ticks) * rndTicks / 2);

            return TimeSpan.FromTicks(ansTicks - (ansTicks % rndTicks));
        }
    }
}
