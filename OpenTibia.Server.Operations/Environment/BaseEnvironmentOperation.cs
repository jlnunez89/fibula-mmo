// -----------------------------------------------------------------
// <copyright file="BaseEnvironmentOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Operations;
    using OpenTibia.Server.Operations.Combat;
    using OpenTibia.Server.Operations.Movements;
    using OpenTibia.Server.Operations.Notifications;
    using OpenTibia.Server.Operations.Notifications.Arguments;
    using Serilog;

    /// <summary>
    /// Class that represents a base environment operation.
    /// </summary>
    public abstract class BaseEnvironmentOperation : ElevatedOperation
    {
        private const decimal MaximumCombatSpeed = 5.0m;

        private const decimal MinimumCombatSpeed = 0.2m;

        /// <summary>
        /// The default exhaustion cost for environment operations.
        /// </summary>
        private static readonly TimeSpan DefaultExhaustionCost = TimeSpan.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEnvironmentOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">A reference to the operation context.</param>
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        /// <param name="actionExhaustionCost">Optional. The cost of this operation. Defaults to <see cref="DefaultExhaustionCost"/>.</param>
        public BaseEnvironmentOperation(ILogger logger, IElevatedOperationContext context, uint requestorId, TimeSpan? actionExhaustionCost = null)
            : base(logger, context, requestorId)
        {
            this.ExhaustionCost = actionExhaustionCost ?? DefaultExhaustionCost;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.None;

        /// <summary>
        /// Gets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; }

        /// <summary>
        /// Attempts to place a creature on the map.
        /// </summary>
        /// <param name="location">The location to place the creature at.</param>
        /// <param name="creature">The creature to place.</param>
        /// <returns>True if the creature is successfully added to the map, false otherwise.</returns>
        protected bool PlaceCreature(Location location, ICreature creature)
        {
            if (location.Type != LocationType.Map)
            {
                return false;
            }

            if (this.Context.TileAccessor.GetTileAt(location, out ITile targetTile))
            {
                var (addResult, _) = targetTile.AddContent(this.Context.ItemFactory, creature);

                if (addResult)
                {
                    this.Context.CreatureManager.RegisterCreature(creature);

                    if (creature is ICombatant combatant)
                    {
                        combatant.TargetChanged += this.HandleCombatantTargetChanged;
                        combatant.ChaseModeChanged += this.HandleCombatantChaseModeChanged;
                        combatant.CombatCreditsConsumed += this.HandleCombatCreditsConsumed;
                    }

                    this.Logger.Debug($"Placed {creature.Name} at {location}.");

                    IEnumerable<IConnection> TargetConnectionsFunc()
                    {
                        if (creature is IPlayer player)
                        {
                            return this.Context.ConnectionManager.PlayersThatCanSee(this.Context.CreatureManager, location).Except(this.Context.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem());
                        }

                        return this.Context.ConnectionManager.PlayersThatCanSee(this.Context.CreatureManager, location);
                    }

                    var placedAtStackPos = targetTile.GetStackPositionOfThing(creature);

                    this.Context.Scheduler.ImmediateEvent(
                            new CreatureMovedNotification(
                            this.Logger,
                            this.Context.MapDescriptor,
                            this.Context.CreatureManager,
                            TargetConnectionsFunc,
                            new CreatureMovedNotificationArguments(creature.Id, default, byte.MaxValue, location, placedAtStackPos, wasTeleport: true)));
                }

                return addResult;
            }

            return false;
        }

        /// <summary>
        /// Attempts to remove a creature from the map.
        /// </summary>
        /// <param name="creature">The creature to remove.</param>
        /// <returns>True if the creature is successfully removed from the map, false otherwise.</returns>
        protected bool RemoveCreature(ICreature creature)
        {
            if (!this.Context.TileAccessor.GetTileAt(creature.Location, out ITile fromTile))
            {
                return false;
            }

            var oldStackpos = fromTile.GetStackPositionOfThing(creature);

            IThing creatureAsThing = creature as IThing;

            var removedFromTile = fromTile.RemoveContent(this.Context.ItemFactory, ref creatureAsThing).result;

            if (removedFromTile)
            {
                this.Context.Scheduler.ImmediateEvent(
                    new CreatureRemovedNotification(
                        this.Logger,
                        this.Context.CreatureManager,
                        () => this.Context.ConnectionFinder.PlayersThatCanSee(this.Context.CreatureFinder, creature.Location),
                        new CreatureRemovedNotificationArguments(creature, oldStackpos, AnimatedEffect.Puff)));
            }

            return removedFromTile;
        }

        protected void HandleCombatCreditsConsumed(ICombatant combatant, CombatCreditType creditType, byte amount)
        {
            if (combatant == null)
            {
                return;
            }

            var restoreOperation = new RestoreCombatCreditOperation(this.Logger, combatant, creditType);

            var normalizedSpeed = Math.Min(MaximumCombatSpeed, Math.Max(MinimumCombatSpeed, creditType == CombatCreditType.Attack ? combatant.BaseAttackSpeed : combatant.BaseDefenseSpeed));
            var restoreCreditDelay = TimeSpan.FromMilliseconds((int)Math.Ceiling(ICombatOperation.DefaultCombatRoundTimeInMs / normalizedSpeed));

            this.Context.Scheduler.ScheduleEvent(restoreOperation, this.Context.Scheduler.CurrentTime + restoreCreditDelay);
        }

        protected void HandleCombatantTargetChanged(ICombatant combatant, ICombatant oldTarget)
        {
            if (combatant == null || combatant.AutoAttackTarget?.Id == oldTarget?.Id)
            {
                return;
            }

            if (combatant.AutoAttackTarget == null)
            {
                // This combatant has stopped attacks.
                combatant.ClearAllLocationActions();
                combatant.ClearAllRangeBasedActions();

                return;
            }

            if (oldTarget != null)
            {
                // This combatant has switched to attack something else.
                combatant.ClearAllLocationActions();
                combatant.ClearAllRangeBasedActions();
            }

            if (combatant.AutoAttackTarget != null)
            {
                var normalizedAttackSpeed = Math.Min(MaximumCombatSpeed, Math.Max(MinimumCombatSpeed, combatant.BaseAttackSpeed));
                var attackCost = TimeSpan.FromMilliseconds((int)Math.Ceiling(ICombatOperation.DefaultCombatRoundTimeInMs / normalizedAttackSpeed));

                IEvent attackOperation = new AutoAttackCombatOperation(this.Logger, this.Context, combatant, combatant.AutoAttackTarget, attackCost);

                var cooldownRemaining = combatant.CalculateRemainingCooldownTime(ExhaustionType.PhysicalCombat, this.Context.Scheduler.CurrentTime);

                this.Context.Scheduler.ScheduleEvent(attackOperation, this.Context.Scheduler.CurrentTime + cooldownRemaining);
            }
        }

        protected void HandleCombatantChaseModeChanged(ICombatant combatant, ChaseMode oldMode)
        {
            if (combatant == null || combatant.ChaseMode == oldMode)
            {
                return;
            }

            if (combatant.ChaseMode == ChaseMode.Chase && combatant.AutoAttackTarget != null)
            {
                var locationDiff = combatant.Location - combatant.AutoAttackTarget.Location;

                if (locationDiff.MaxValueIn2D > 1)
                {
                    this.Context.Scheduler.CancelAllFor(combatant.Id, typeof(IMovementOperation));

                    var directions = this.Context.PathFinder.FindBetween(combatant.Location, combatant.AutoAttackTarget.Location, out _, onBehalfOfCreature: combatant, considerAvoidsAsBlock: true);

                    if (directions != null && directions.Any())
                    {
                        this.AutoWalk(combatant, directions.ToArray());
                    }
                }
            }
        }

        protected void HandlePlayerInventoryChanged(IInventory inventory, Slot slot, IItem item)
        {
            if (!(inventory.Owner is IPlayer player))
            {
                return;
            }

            this.Logger.Information($"{player.Name}'s inventory slot {slot} changed to {item?.ToString() ?? "empty"}.");

            var notificationArgs = item == null ?
                new GenericNotificationArguments(new PlayerInventoryClearSlotPacket(slot))
                :
                new GenericNotificationArguments(new PlayerInventorySetSlotPacket(slot, item));

            var notification = new GenericNotification(this.Logger, () => this.Context.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem(), notificationArgs);

            this.Context.Scheduler.ImmediateEvent(notification);
        }

        /// <summary>
        /// Attempts to schedule a creature's auto walk movements.
        /// </summary>
        /// <param name="creature">The creature making the request.</param>
        /// <param name="directions">The directions to walk to.</param>
        /// <param name="stepIndex">Optional. The index in the directions array at which to start moving. Defaults to zero.</param>
        /// <returns>True if the auto walk request was accepted, false otherwise.</returns>
        private bool AutoWalk(ICreature creature, Direction[] directions, int stepIndex = 0)
        {
            creature.ThrowIfNull(nameof(creature));

            if (directions.Length == 0 || stepIndex >= directions.Length)
            {
                return true;
            }

            // A new request overrides and cancels any "auto" actions waiting to be retried.
            this.Context.Scheduler.CancelAllFor(creature.Id, typeof(IMovementOperation));

            var nextLocation = creature.Location.LocationAt(directions[stepIndex]);

            TimeSpan movementDelay = creature.CalculateRemainingCooldownTime(ExhaustionType.Movement, this.Context.Scheduler.CurrentTime);

            IEvent movementOperation = new MapToMapMovementOperation(this.Logger, this.Context, creature.Id, creature, creature.Location, nextLocation);

            this.Context.Scheduler.ScheduleEvent(movementOperation, this.Context.Scheduler.CurrentTime + movementDelay);

            if (directions.Length > 1)
            {
                // Add this request as the retry action, so that the request gets repeated when the player hits this location.
                creature.EnqueueRetryActionAtLocation(nextLocation, () => this.AutoWalk(creature, directions, stepIndex + 1));
            }

            return true;
        }
    }
}