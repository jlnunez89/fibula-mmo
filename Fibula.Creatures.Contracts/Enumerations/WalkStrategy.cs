// -----------------------------------------------------------------
// <copyright file="WalkStrategy.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Enumerations
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
