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
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using OpenTibia.Server.Operations;
    using OpenTibia.Server.Operations.Combat;
    using Serilog;

    /// <summary>
    /// Class that represents a base environment operation.
    /// </summary>
    public abstract class BaseEnvironmentOperation : ElevatedOperation
    {
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
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Attempts to place a creature on the map.
        /// </summary>
        /// <param name="targetTile">The tile to place the creature at.</param>
        /// <param name="creature">The creature to place.</param>
        /// <returns>True if the creature is successfully added to the map, false otherwise.</returns>
        protected bool PlaceCreature(ITile targetTile, ICreature creature)
        {
            targetTile.ThrowIfNull(nameof(targetTile));
            creature.ThrowIfNull(nameof(creature));

            var (addSuccessful, _) = targetTile.AddContent(this.Context.ItemFactory, creature);

            if (addSuccessful)
            {
                this.Context.CreatureManager.RegisterCreature(creature);

                if (creature is ICombatant combatant)
                {
                    combatant.TargetChanged += this.OnCombatantTargetChanged;
                    combatant.ChaseModeChanged += this.OnCombatantChaseModeChanged;
                    combatant.CombatCreditsConsumed += this.OnCombatCreditsConsumed;
                }

                if (creature is IPlayer player)
                {
                    player.Inventory.SlotChanged += this.OnPlayerInventoryChanged;
                }

                this.Logger.Debug($"Placed {creature.Name} at {targetTile.Location}.");

                IEnumerable<IConnection> TargetConnectionsFunc()
                {
                    if (creature is IPlayer player)
                    {
                        return this.Context.ConnectionManager.PlayersThatCanSee(this.Context.CreatureManager, targetTile.Location).Except(this.Context.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem());
                    }

                    return this.Context.ConnectionManager.PlayersThatCanSee(this.Context.CreatureManager, targetTile.Location);
                }

                var placedAtStackPos = targetTile.GetStackPositionOfThing(creature);

                this.Context.Scheduler.ScheduleEvent(
                        new CreatureMovedNotification(
                        this.Logger,
                        this.Context.MapDescriptor,
                        this.Context.CreatureManager,
                        TargetConnectionsFunc,
                        new CreatureMovedNotificationArguments(creature.Id, default, byte.MaxValue, targetTile.Location, placedAtStackPos, wasTeleport: true)));
            }

            return addSuccessful;
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
                if (creature is ICombatant combatant)
                {
                    combatant.TargetChanged -= this.OnCombatantTargetChanged;
                    combatant.ChaseModeChanged -= this.OnCombatantChaseModeChanged;
                    combatant.CombatCreditsConsumed -= this.OnCombatCreditsConsumed;
                }

                if (creature is IPlayer player)
                {
                    player.Inventory.SlotChanged -= this.OnPlayerInventoryChanged;
                }

                this.Context.Scheduler.ScheduleEvent(
                    new CreatureRemovedNotification(
                        this.Logger,
                        this.Context.CreatureManager,
                        () => this.Context.ConnectionFinder.PlayersThatCanSee(this.Context.CreatureFinder, creature.Location),
                        new CreatureRemovedNotificationArguments(creature, oldStackpos, AnimatedEffect.Puff)));
            }

            return removedFromTile;
        }

        private void OnCombatCreditsConsumed(ICombatant combatant, CombatCreditType creditType, byte amount)
        {
            if (combatant == null)
            {
                return;
            }

            var combatSpeed = creditType == CombatCreditType.Attack ? combatant.BaseAttackSpeed : combatant.BaseDefenseSpeed;
            var restoreOperation = new RestoreCombatCreditOperation(this.Logger, this.Context, combatant, creditType);
            var restoreCreditDelay = TimeSpan.FromMilliseconds((int)Math.Ceiling(ICombatOperation.DefaultCombatRoundTimeInMs / combatSpeed));

            this.Context.Scheduler.ScheduleEvent(restoreOperation, restoreCreditDelay);
        }

        private void OnCombatantTargetChanged(ICombatant combatant, ICombatant oldTarget)
        {
            if (combatant?.AutoAttackTarget == null)
            {
                // This combatant has stopped attacks.
                return;
            }

            var attackExhaustionCost = TimeSpan.FromMilliseconds((int)Math.Ceiling(ICombatOperation.DefaultCombatRoundTimeInMs / combatant.BaseAttackSpeed));

            var attackOperation = new AutoAttackCombatOperation(this.Logger, this.Context, combatant, combatant.AutoAttackTarget, attackExhaustionCost);

            var cooldownRemaining = combatant.CalculateRemainingCooldownTime(attackOperation.ExhaustionType, this.Context.Scheduler.CurrentTime);

            this.Context.Scheduler.ScheduleEvent(attackOperation, cooldownRemaining);
        }

        private void OnCombatantChaseModeChanged(ICombatant combatant, ChaseMode oldMode)
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

                    if (directions == null || !directions.Any())
                    {
                        this.SendFailureNotification(OperationMessage.ThereIsNoWay);
                    }
                    else
                    {
                        this.AutoWalk(combatant, directions.ToArray());
                    }
                }
            }
        }

        private void OnPlayerInventoryChanged(IInventory inventory, Slot slot, IItem item)
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

            this.Context.Scheduler.ScheduleEvent(notification);
        }
    }
}