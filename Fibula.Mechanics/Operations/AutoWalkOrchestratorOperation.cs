// -----------------------------------------------------------------
// <copyright file="AutoWalkOrchestratorOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Constants;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Extensions;
    using Fibula.Mechanics.Operations.Arguments;

    /// <summary>
    /// Class that represents an operation that orchestrates auto walk operations.
    /// </summary>
    public class AutoWalkOrchestratorOperation : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoWalkOrchestratorOperation"/> class.
        /// </summary>
        /// <param name="creature">The creature that is auto walking.</param>
        public AutoWalkOrchestratorOperation(ICreature creature)
            : base(creature?.Id ?? 0)
        {
            creature.ThrowIfNull(nameof(creature));

            this.Creature = creature;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.None;

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Gets the combatant that is attacking on this operation.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            if (this.Creature == null || this.Creature.IsDead)
            {
                this.RepeatAfter = TimeSpan.Zero;

                return;
            }

            var resultingState = this.Creature.WalkPlan.UpdateState(this.Creature.Location);

            if (this.Creature.WalkPlan.IsInTerminalState)
            {
                this.RepeatAfter = TimeSpan.Zero;

                return;
            }

            // Recalculate the route if necessary:
            if (resultingState == WalkPlanState.NeedsToRecalculate)
            {
                var directions = context.PathFinder.FindBetween(this.Creature.Location, this.Creature.WalkPlan.DetermineGoal(), out Location endLocation, this.Creature);

                this.Creature.WalkPlan.RecalculateWaypoints(this.Creature.Location, directions);
            }

            // Pop the current waypoint.
            this.Creature.WalkPlan.Waypoints.RemoveFirst();

            // If even after recalculating we're left without waypoints, we are done.
            if (this.Creature.WalkPlan.Waypoints.Count == 0)
            {
                this.RepeatAfter = TimeSpan.Zero;

                return;
            }

            var nextLocation = this.Creature.WalkPlan.Waypoints.First.Value;
            var scheduleDelay = TimeSpan.Zero;

            var autoWalkOp = context.OperationFactory.Create(
                new MovementOperationCreationArguments(
                    this.Creature.Id,
                    CreatureConstants.CreatureThingId,
                    this.Creature.Location,
                    byte.MaxValue,
                    this.Creature.Id,
                    nextLocation,
                    this.Creature.Id));

            // Add delay from current exhaustion of the requestor, if any.
            if (this.Creature is ICreatureWithExhaustion creatureWithExhaustion)
            {
                // The scheduling delay becomes any cooldown debt for this operation.
                scheduleDelay = creatureWithExhaustion.CalculateRemainingCooldownTime(autoWalkOp.ExhaustionType, context.Scheduler.CurrentTime);
            }

            // Schedule the actual walk operation.
            context.Scheduler.ScheduleEvent(autoWalkOp, scheduleDelay);

            this.RepeatAfter = this.Creature.CalculateStepDuration(this.Creature.Location.DirectionTo(nextLocation), context.Map.GetTileAt(this.Creature.Location));
        }
    }
}
