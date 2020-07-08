// -----------------------------------------------------------------
// <copyright file="WalkStrategy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Enumerations
{
    /// <summary>
    /// Enumeration of the possible walking strategies.
    /// </summary>
    public enum WalkStrategy
    {
        /// <summary>
        /// Intended for static goals.
        /// When a creature is following a walk plan and it's suddenly interrupted,
        /// it will give up and abort the plan, sending a walk cancellation if applicable.
        /// </summary>
        DoNotRecalculate,

        /// <summary>
        /// Intended for static goals.
        /// When a creature is following a walk plan and it's suddenly interrupted,
        /// it will recalculate the plan from it's current position.
        /// </summary>
        RecalculateOnInterruption,

        /// <summary>
        /// Intended for slow moving goals, or low level artificial inteligence.
        /// The creature follows the walk plan and there is a 10% chance of recalculating
        /// at each waypoint. If the waypoint is not accessible or the creature is otherwise
        /// interrupted, it will recalculate the plan from it's current position.
        /// </summary>
        ConservativeRecalculation,

        /// <summary>
        /// Intended for moderate moving goals, following players, or medium artificial inteligence.
        /// The creature follows the walk plan and there is a 25% chance of recalculating
        /// at each waypoint. If the waypoint is not accessible or the creature is otherwise
        /// interrupted, it will recalculate the plan from it's current position.
        /// </summary>
        AggresiveRecalculation,

        /// <summary>
        /// Intended for fast moving goals, or highest artificial inteligence. Compute expensive.
        /// The creature follows the walk plan and there is a 50% chance of recalculating
        /// at each waypoint. If the waypoint is not accessible or the creature is otherwise
        /// interrupted, it will recalculate the plan from it's current position.
        /// </summary>
        ExtremeRecalculation,
    }
}
