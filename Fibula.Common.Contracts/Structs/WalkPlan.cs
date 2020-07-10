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

namespace Fibula.Common.Contracts
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;

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

            this.State = WalkPlanState.NeedsToRecalculate;

            this.Rng = new Random();
        }

        /// <summary>
        /// Gets the strategy of this walk plan.
        /// </summary>
        public WalkStrategy Strategy { get; }

        /// <summary>
        /// Gets or sets the state of this walk plan.
        /// </summary>
        public WalkPlanState State { get; set; }

        /// <summary>
        /// Gets the function that determines the goal location.
        /// </summary>
        public Func<Location> DetermineGoal { get; }

        /// <summary>
        /// Gets the waypoints of this walk plan.
        /// </summary>
        public LinkedList<Location> Waypoints { get; }

        /// <summary>
        /// Gets a value indicating whether the walk plan is in a terminal state.
        /// </summary>
        public bool IsInTerminalState => this.State == WalkPlanState.Aborted || this.State == WalkPlanState.AtGoal;

        /// <summary>
        /// Gets or sets the pseudo-random number generator to work with.
        /// </summary>
        public Random Rng { get; set; }
    }
}
