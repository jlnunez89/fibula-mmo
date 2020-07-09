// -----------------------------------------------------------------
// <copyright file="WalkPlanExtensions.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using Fibula.Common.Contracts;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;

    /// <summary>
    /// Helper class for extension methods of walk plans.
    /// </summary>
    public static class WalkPlanExtensions
    {
        /// <summary>
        /// Checks and updates the plan by comparing the current waypoint's location to the plan's waypoints.
        /// This also accounts for the plan's strategy and flags to recalculate as per the <see cref="WalkStrategy"/> rules.
        /// </summary>
        /// <param name="walkPlan">The walkplan to check.</param>
        /// <param name="currentWaypoint">The current waypoint.</param>
        /// <returns>The resulting state of the walk plan..</returns>
        public static WalkPlanState UpdateState(this WalkPlan walkPlan, Location currentWaypoint)
        {
            if (walkPlan.IsInTerminalState)
            {
                return walkPlan.State;
            }

            // The currentLocation must be either the goal, or the first location on our waypoints list.
            if (currentWaypoint == walkPlan.DetermineGoal())
            {
                walkPlan.State = WalkPlanState.AtGoal;

                return WalkPlanState.AtGoal;
            }

            // Check if we're on track.
            if (walkPlan.Waypoints.Count > 0 && currentWaypoint == walkPlan.Waypoints.First.Value)
            {
                walkPlan.State = WalkPlanState.OnTrack;
            }

            // We'll need recalculation even if we're on track if we haven't reached our goal yet and there's one
            // last waypoint left, meaning it's a moving goal.
            var needsRecalculation = walkPlan.Waypoints.Count <= 1;

            // Also, if the strategy has a chance to, flag for recalculation.
            switch (walkPlan.Strategy)
            {
                case WalkStrategy.ConservativeRecalculation:
                    // 10% chance of recalculation.
                    needsRecalculation |= walkPlan.Rng.Next(10) == 0;

                    break;
                case WalkStrategy.AggresiveRecalculation:
                    // 25% chance of recalculation.
                    needsRecalculation |= walkPlan.Rng.Next(4) == 0;

                    break;
                case WalkStrategy.ExtremeRecalculation:
                    // 50% chance of recalculation.
                    needsRecalculation |= walkPlan.Rng.Next(2) == 0;

                    break;
            }

            if (needsRecalculation)
            {
                // Of course, only in case we're not against recalculation.
                if (walkPlan.Strategy == WalkStrategy.DoNotRecalculate)
                {
                    walkPlan.State = WalkPlanState.Aborted;
                }
                else
                {
                    walkPlan.State = WalkPlanState.NeedsToRecalculate;
                }
            }

            return walkPlan.State;
        }

        /// <summary>
        /// Recalculates the plan's waypoints (locations) given a start location and some directions.
        /// </summary>
        /// <param name="walkPlan">The walkplan to recalculate for.</param>
        /// <param name="startLocation">The start location.</param>
        /// <param name="directions">The directions to create the waypoints with.</param>
        public static void RecalculateWaypoints(this WalkPlan walkPlan, Location startLocation, IEnumerable<Direction> directions)
        {
            if (directions == null)
            {
                return;
            }

            walkPlan.State = WalkPlanState.OnTrack;
            walkPlan.Waypoints.Clear();
            walkPlan.Waypoints.AddFirst(startLocation);

            var lastLoc = startLocation;

            // Calculate and add the waypoints.
            foreach (var dir in directions)
            {
                var nextLoc = lastLoc.LocationAt(dir);

                walkPlan.Waypoints.AddLast(nextLoc);

                lastLoc = nextLoc;
            }
        }
    }
}
