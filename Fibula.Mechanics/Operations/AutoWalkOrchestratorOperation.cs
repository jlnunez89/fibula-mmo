// -----------------------------------------------------------------
// <copyright file="AutoWalkOrchestratorOperation.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Extensions;
    using Fibula.Common.Utilities;
    using Fibula.Common.Utilities.Pathfinding;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Constants;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Extensions;
    using Fibula.Mechanics.Notifications;

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
            if (this.Creature.IsDead)
            {
                return;
            }

            this.Creature.WalkPlan.Checkpoint(this.Creature.Location);

            if (this.Creature.WalkPlan.State == WalkPlanState.Aborted)
            {
                if (this.Creature is IPlayer player)
                {
                    this.SendNotification(
                        context,
                        new GenericNotification(
                            () => player.YieldSingleItem(),
                            new PlayerCancelWalkPacket(player.Direction.GetClientSafeDirection())));
                }
            }

            // Recalculate the route if necessary.
            if (this.Creature.WalkPlan.State == WalkPlanState.NeedsToRecalculate)
            {
                var (result, _, directions) = context.PathFinder.FindBetween(this.Creature.Location, this.Creature.WalkPlan.GetGoalLocation(), this.Creature, targetDistance: this.Creature.WalkPlan.GoalTargetDistance);

                if (result == SearchState.Failed)
                {
                    // No way found.
                    this.DispatchTextNotification(context, OperationMessage.ThereIsNoWay);

                    if (this.Creature is Player player)
                    {
                        // For players, we stop trying and revert to standing mode. This also sets the following target to null.
                        context.CombatApi.SetCombatantModes(player, player.FightMode, ChaseMode.Stand, false /*combatant.HasSafetyOn*/);
                    }
                    else if (this.Creature is Monster monster)
                    {
                        // For monsters however, we choose a random movement to make.
                        directions = new List<Direction>
                        {
                            // TODO: this should only pick from non-avoided tiles.
                            monster.RandomAdjacentDirection(),
                        };

                        monster.LastMovementCostModifier = 3;
                    }
                }

                if (directions.Any())
                {
                    this.Creature.WalkPlan.RecalculateWaypoints(this.Creature.Location, directions);
                }
            }

            if (this.Creature.WalkPlan.State == WalkPlanState.OnTrack)
            {
                // Consume the next waypoint since it's the one we're standing on.
                this.Creature.WalkPlan.Waypoints.RemoveFirst();
            }

            // If we're left without waypoints, we can't continue.
            if (this.Creature.WalkPlan.Waypoints.Count == 0)
            {
                // But if it's a non-static goal, we need to wait and repeat.
                if (!this.Creature.WalkPlan.Strategy.IsStatic())
                {
                    this.RepeatAfter = this.Creature.WalkPlan.WaitTime;
                }

                return;
            }

            var nextLocation = this.Creature.WalkPlan.Waypoints.First.Value;
            var scheduleDelay = TimeSpan.Zero;

            var autoWalkOp = new MovementOperation(
                    this.Creature.Id,
                    CreatureConstants.CreatureTypeId,
                    this.Creature.Location,
                    byte.MaxValue,
                    this.Creature.Id,
                    nextLocation,
                    this.Creature.Id,
                    amount: 1);

            // Add delay from current exhaustion of the requestor, if any.
            if (this.Creature is ICreatureWithExhaustion creatureWithExhaustion)
            {
                // The scheduling delay becomes any cooldown debt for this operation.
                scheduleDelay = creatureWithExhaustion.CalculateRemainingCooldownTime(autoWalkOp.ExhaustionType, context.Scheduler.CurrentTime);
            }

            // Schedule the actual walk operation.
            context.Scheduler.ScheduleEvent(autoWalkOp, scheduleDelay);

            this.RepeatAfter = this.Creature.CalculateStepDuration(context.Map.GetTileAt(this.Creature.Location));
        }
    }
}
