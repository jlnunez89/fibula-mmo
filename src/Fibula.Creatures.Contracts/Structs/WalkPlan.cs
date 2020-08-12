// -----------------------------------------------------------------
// <copyright file="WalkPlan.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Structs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a creature's walk plan.
    /// </summary>
    public class WalkPlan
    {
        /// <summary>
        /// Stores the current number of consecutive recalculations that the plan
        /// has observed when the <see cref="Checkpoint(Location, Random)"/> method is run.
        /// </summary>
        private uint consecutiveRecalculations;

        /// <summary>
        /// Initializes a new instance of the <see cref="WalkPlan"/> class.
        /// </summary>
        /// <param name="strategy">The strategy of the plan.</param>
        /// <param name="goalDeterminationFunction">A function used to determine the goal of this plan.</param>
        /// <param name="goalDistance">The distance to consider the walk plan at goal.</param>
        /// <param name="startingWaypoints">The waypoints already known to the goal.</param>
        public WalkPlan(WalkPlanStrategy strategy, Func<Location> goalDeterminationFunction, int goalDistance = 1, params Location[] startingWaypoints)
        {
            goalDeterminationFunction.ThrowIfNull(nameof(goalDeterminationFunction));

            this.Strategy = strategy;
            this.GetGoalLocation = goalDeterminationFunction;
            this.GoalTargetDistance = goalDistance;

            this.Waypoints = new LinkedList<Location>(startingWaypoints);

            this.consecutiveRecalculations = 0;
            this.State = this.Waypoints.Count > 0 ? WalkPlanState.OnTrack : WalkPlanState.NeedsToRecalculate;
        }

        /// <summary>
        /// Gets the strategy of this walk plan.
        /// </summary>
        public WalkPlanStrategy Strategy { get; }

        /// <summary>
        /// Gets or sets the state of this walk plan.
        /// </summary>
        public WalkPlanState State { get; set; }

        /// <summary>
        /// Gets the function that determines the goal location, used when requesting recalculation of the path to the goal.
        /// </summary>
        public Func<Location> GetGoalLocation { get; }

        /// <summary>
        /// Gets the target distance to use when requesting recalculation of the path to the goal.
        /// </summary>
        public int GoalTargetDistance { get; }

        /// <summary>
        /// Gets the waypoints of this walk plan.
        /// </summary>
        public LinkedList<Location> Waypoints { get; }

        /// <summary>
        /// Gets the recommended wait time based on the last <see cref="Checkpoint(Location, Random)"/> run.
        /// </summary>
        public TimeSpan WaitTime => this.State switch
        {
            WalkPlanState.Aborted => TimeSpan.MinValue,
            WalkPlanState.NeedsToRecalculate => this.GetWaitTime(),
            _ => TimeSpan.Zero
        };

        /// <summary>
        /// Steps into the next waypoint in the plan by comparing it to the current location and updating its state.
        /// i.e. this flags the plan to recalculate as per the <see cref="WalkPlanStrategy"/> rules.
        /// </summary>
        /// <param name="currentLocation">The current location to check against.</param>
        /// <param name="rng">Optional. A pseudo-random number generator to work with.</param>
        public void Checkpoint(Location currentLocation, Random rng = null)
        {
            if (rng == null)
            {
                rng = new Random();
            }

            var oldState = this.State;

            // First, check if the plan is aborted, as we can't update it if so.
            if (this.State == WalkPlanState.Aborted)
            {
                this.Waypoints.Clear();

                return;
            }

            // Check if we're on track (i.e. at the first waypoint in our list).
            if (this.Waypoints.FirstOrDefault() == currentLocation)
            {
                this.State = WalkPlanState.OnTrack;
            }

            // Now check if we need recalculation. This happens if we're out waypoints,
            // or if the strategy has a chance to do it.
            var needsRecalculation = this.Waypoints.Count == 0;

            switch (this.Strategy)
            {
                case WalkPlanStrategy.ConservativeRecalculation:
                    // 33% chance of recalculation.
                    needsRecalculation |= rng.Next(3) == 0;

                    break;
                case WalkPlanStrategy.AggressiveRecalculation:
                    // 50% chance of recalculation.
                    needsRecalculation |= rng.Next(2) == 0;

                    break;
                case WalkPlanStrategy.ExtremeRecalculation:
                    // 75% chance of recalculation.
                    needsRecalculation |= rng.Next(4) != 0;

                    break;
            }

            if (needsRecalculation)
            {
                // Of course, only in case we're not against recalculation.
                if (this.Strategy == WalkPlanStrategy.DoNotRecalculate)
                {
                    this.State = WalkPlanState.Aborted;
                }
                else
                {
                    this.State = WalkPlanState.NeedsToRecalculate;
                }
            }

            // Update the internal consecutive recalculation counter to weight it in correctly on wait time calculations.
            if (this.State == WalkPlanState.NeedsToRecalculate)
            {
                if (oldState == this.State)
                {
                    this.consecutiveRecalculations++;
                }
            }
            else
            {
                this.consecutiveRecalculations = 0;
            }
        }

        /// <summary>
        /// Recalculates the plan's waypoints (locations) given a start location and some directions.
        /// </summary>
        /// <param name="startLocation">The start location.</param>
        /// <param name="directions">The directions to create the waypoints with.</param>
        public void RecalculateWaypoints(Location startLocation, IEnumerable<Direction> directions)
        {
            if (directions == null)
            {
                return;
            }

            this.State = WalkPlanState.OnTrack;
            this.Waypoints.Clear();
            this.Waypoints.AddFirst(startLocation);

            var lastLoc = startLocation;

            // Calculate and add the waypoints.
            foreach (var dir in directions)
            {
                var nextLoc = lastLoc.LocationAt(dir);

                this.Waypoints.AddLast(nextLoc);

                lastLoc = nextLoc;
            }
        }

        private TimeSpan GetWaitTime()
        {
            var maxMillisecondsPerStrategy = this.Strategy switch
            {
                WalkPlanStrategy.ExtremeRecalculation => 500,
                WalkPlanStrategy.AggressiveRecalculation => 1500,
                WalkPlanStrategy.ConservativeRecalculation => 2500,
                _ => 4000
            };

            var milliseconds = Math.Min(maxMillisecondsPerStrategy, this.consecutiveRecalculations * 500);

            return TimeSpan.FromMilliseconds(milliseconds);
        }
    }
}
