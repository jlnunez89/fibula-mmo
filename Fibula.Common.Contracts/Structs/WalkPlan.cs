// -----------------------------------------------------------------
// <copyright file="WalkPlan.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Microsoft.EntityFrameworkCore.Internal;

    /// <summary>
    /// Class that represents a walk plan.
    /// </summary>
    public struct WalkPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WalkPlan"/> struct.
        /// </summary>
        /// <param name="strategy">The strategy of the plan.</param>
        /// <param name="goalDeterminationFunction">A function used to determine the goal of this plan.</param>
        /// <param name="startingWaypoints">The waypoints already known to the goal.</param>
        public WalkPlan(WalkStrategy strategy, Func<Location> goalDeterminationFunction, params Location[] startingWaypoints)
        {
            this.Strategy = strategy;
            this.DetermineGoal = goalDeterminationFunction;
            this.Waypoints = new LinkedList<Location>(startingWaypoints);

            this.State = WalkPlanState.InProgress;
        }

        /// <summary>
        /// Gets the strategy of this walk plan.
        /// </summary>
        public WalkStrategy Strategy { get; }

        /// <summary>
        /// Gets the state of this walk plan.
        /// </summary>
        public WalkPlanState State { get; private set; }

        /// <summary>
        /// Gets the function that determines the goal location.
        /// </summary>
        public Func<Location> DetermineGoal { get; }

        /// <summary>
        /// Gets the waypoints of this walk plan.
        /// </summary>
        public LinkedList<Location> Waypoints { get; }

        /// <summary>
        /// Checks if the plan is going as intended, by comparing the current location to
        /// the waypoints of the plan.
        /// </summary>
        /// <param name="currentLocation">The current location.</param>
        /// <returns>True if the current location is expected, false otherwise, or if the plan is out of waypoints.</returns>
        public bool GoingAsIntended(Location currentLocation)
        {
            // The currentLocation must be either the goal, or the next location on our waypoints.
            if (currentLocation == this.DetermineGoal())
            {
                this.State = WalkPlanState.AtGoal;
                this.Waypoints.Clear();

                return true;
            }

            if (this.Waypoints.Count > 0 && currentLocation == this.Waypoints.First.Value)
            {
                // Pop the next node and return, we're on the right path.
                this.Waypoints.RemoveFirst();

                return true;
            }

            if (this.Waypoints.Count == 0)
            {
                // We're out of waypoints and we're not at our goal...
                // If we're giving up, mark as aborted before returning.
                if (this.Strategy == WalkStrategy.GiveUpOnInterruption)
                {
                    this.State = WalkPlanState.Aborted;
                }
            }

            return false;
        }
    }
}
