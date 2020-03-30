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
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using OpenTibia.Server.Operations;

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
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        /// <param name="actionExhaustionCost">Optional. The cost of this operation. Defaults to <see cref="DefaultExhaustionCost"/>.</param>
        public BaseEnvironmentOperation(uint requestorId, TimeSpan? actionExhaustionCost = null)
            : base(requestorId)
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
        /// <param name="context">A reference to the operation context.</param>
        /// <param name="targetTile">The tile to place the creature at.</param>
        /// <param name="creature">The creature to place.</param>
        /// <returns>True if the creature is successfully added to the map, false otherwise.</returns>
        protected bool PlaceCreature(IElevatedOperationContext context, ITile targetTile, ICreature creature)
        {
            targetTile.ThrowIfNull(nameof(targetTile));
            creature.ThrowIfNull(nameof(creature));

            var (addSuccessful, _) = targetTile.AddContent(context.ItemFactory, creature);

            if (addSuccessful)
            {
                context.CreatureManager.RegisterCreature(creature);

                if (creature is ICombatant combatant)
                {
                    combatant.CombatStarted += context.CombatApi.OnCombatantCombatStarted;
                    combatant.CombatEnded += context.CombatApi.OnCombatantCombatEnded;
                    combatant.TargetChanged += context.CombatApi.OnCombatantTargetChanged;
                    combatant.ChaseModeChanged += context.CombatApi.OnCombatantChaseModeChanged;
                    combatant.CombatCreditsConsumed += context.CombatApi.OnCombatCreditsConsumed;
                }

                if (creature is IPlayer player)
                {
                    player.Inventory.SlotChanged += context.GameApi.OnPlayerInventoryChanged;
                }

                context.Logger.Debug($"Placed {creature.Name} at {targetTile.Location}.");

                IEnumerable<IConnection> TargetConnectionsFunc()
                {
                    if (creature is IPlayer player)
                    {
                        return context.ConnectionManager.PlayersThatCanSee(context.CreatureManager, targetTile.Location).Except(context.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem());
                    }

                    return context.ConnectionManager.PlayersThatCanSee(context.CreatureManager, targetTile.Location);
                }

                var placedAtStackPos = targetTile.GetStackPositionOfThing(creature);

                context.Scheduler.ScheduleEvent(
                    new CreatureMovedNotification(
                        context.MapDescriptor,
                        context.CreatureManager,
                        TargetConnectionsFunc,
                        new CreatureMovedNotificationArguments(creature.Id, default, byte.MaxValue, targetTile.Location, placedAtStackPos, wasTeleport: true)));
            }

            return addSuccessful;
        }

        /// <summary>
        /// Attempts to remove a creature from the map.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        /// <param name="creature">The creature to remove.</param>
        /// <returns>True if the creature is successfully removed from the map, false otherwise.</returns>
        protected bool RemoveCreature(IElevatedOperationContext context, ICreature creature)
        {
            if (!context.TileAccessor.GetTileAt(creature.Location, out ITile fromTile))
            {
                return false;
            }

            var oldStackpos = fromTile.GetStackPositionOfThing(creature);

            IThing creatureAsThing = creature as IThing;

            var removedFromTile = fromTile.RemoveContent(context.ItemFactory, ref creatureAsThing).result;

            if (removedFromTile)
            {
                if (creature is ICombatant combatant)
                {
                    combatant.CombatStarted -= context.CombatApi.OnCombatantCombatStarted;
                    combatant.CombatEnded -= context.CombatApi.OnCombatantCombatEnded;
                    combatant.TargetChanged -= context.CombatApi.OnCombatantTargetChanged;
                    combatant.ChaseModeChanged -= context.CombatApi.OnCombatantChaseModeChanged;
                    combatant.CombatCreditsConsumed -= context.CombatApi.OnCombatCreditsConsumed;
                }

                if (creature is IPlayer player)
                {
                    player.Inventory.SlotChanged -= context.GameApi.OnPlayerInventoryChanged;
                }

                context.Scheduler.ScheduleEvent(
                    new CreatureRemovedNotification(
                        context.CreatureManager,
                        () => context.ConnectionFinder.PlayersThatCanSee(context.CreatureFinder, creature.Location),
                        new CreatureRemovedNotificationArguments(creature, oldStackpos, AnimatedEffect.Puff)));
            }

            return removedFromTile;
        }
    }
}