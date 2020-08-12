// -----------------------------------------------------------------
// <copyright file="BaseEnvironmentOperation.cs" company="2Dudes">
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
    using System.Linq;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Notifications;

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

        ///// <summary>
        ///// Gets the type of exhaustion that this operation produces.
        ///// </summary>
        // public override ExhaustionType ExhaustionType => ExhaustionType.None;

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
                    combatant.StatChanged += context.GameApi.CreatureStatChanged;

                    combatant.Death += context.CombatApi.CombatantDeath;
                    combatant.AttackTargetChanged += context.CombatApi.CombatantAttackTargetChanged;
                    combatant.FollowTargetChanged += context.CombatApi.CreatureFollowTargetChanged;
                    combatant.LocationChanged += context.GameApi.AfterThingLocationChanged;

                    // As a creature that can sense.
                    combatant.CreatureSeen += context.CombatApi.CreatureHasSeenCreature;
                    combatant.CreatureLost += context.CombatApi.CreatureHasLostCreature;

                    // As a skilled creature
                    combatant.SkillChanged += context.GameApi.SkilledCreatureSkillChanged;

                    // Find now-spectators of this creature to start tracking that guy.
                    foreach (var spectator in context.Map.CreaturesThatCanSee(creature.Location))
                    {
                        if (spectator is ICombatant combatantSpectator)
                        {
                            combatant.StartSensingCreature(combatantSpectator);
                            combatantSpectator.StartSensingCreature(combatant);
                        }
                    }
                }

                /*
                if (creature is IPlayer player)
                {
                    player.Inventory.SlotChanged += context.GameApi.OnPlayerInventoryChanged;
                }
                */

                context.Logger.Debug($"Placed {creature.Name} at {targetTile.Location}.");

                var placedAtStackPos = targetTile.GetStackOrderOfThing(creature);

                this.SendNotification(
                    context,
                    new CreatureMovedNotification(
                        () =>
                        {
                            if (creature is IPlayer player)
                            {
                                return context.Map.PlayersThatCanSee(creature.Location).Except(player.YieldSingleItem());
                            }

                            return context.Map.PlayersThatCanSee(creature.Location);
                        },
                        creature.Id,
                        default,
                        byte.MaxValue,
                        creature.Location,
                        placedAtStackPos,
                        wasTeleport: true));
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
            if (!context.Map.GetTileAt(creature.Location, out ITile fromTile))
            {
                return false;
            }

            var oldStackpos = fromTile.GetStackOrderOfThing(creature);

            IThing creatureAsThing = creature;

            var removedFromTile = fromTile.RemoveContent(context.ItemFactory, ref creatureAsThing).result;

            if (removedFromTile)
            {
                if (creature is ICombatant combatant)
                {
                    combatant.SetAttackTarget(null);

                    foreach (var attacker in combatant.AttackedBy)
                    {
                        attacker.StopSensingCreature(combatant);
                    }

                    foreach (var trackedCreature in combatant.TrackedCreatures)
                    {
                        combatant.StopSensingCreature(trackedCreature);

                        if (trackedCreature is ICreatureThatSensesOthers creatureThatSensesOthers)
                        {
                            creatureThatSensesOthers.StopSensingCreature(combatant);
                        }
                    }

                    combatant.StatChanged -= context.GameApi.CreatureStatChanged;

                    combatant.Death -= context.CombatApi.CombatantDeath;
                    combatant.AttackTargetChanged -= context.CombatApi.CombatantAttackTargetChanged;
                    combatant.FollowTargetChanged -= context.CombatApi.CreatureFollowTargetChanged;
                    combatant.LocationChanged -= context.GameApi.AfterThingLocationChanged;

                    // As a creature that can sense.
                    combatant.CreatureSeen -= context.CombatApi.CreatureHasSeenCreature;
                    combatant.CreatureLost -= context.CombatApi.CreatureHasLostCreature;

                    // As a skilled creature
                    combatant.SkillChanged -= context.GameApi.SkilledCreatureSkillChanged;
                }

                if (creature is IPlayer player)
                {
                    this.SendNotification(
                        context,
                        new CreatureRemovedNotification(
                            () => context.Map.PlayersThatCanSee(creature.Location).Union(player.YieldSingleItem()),
                            creature,
                            oldStackpos));

                    // player.Inventory.SlotChanged -= context.GameApi.OnPlayerInventoryChanged;
                }
                else
                {
                    this.SendNotification(context, new CreatureRemovedNotification(() => context.Map.PlayersThatCanSee(creature.Location), creature, oldStackpos));
                }
            }

            return removedFromTile;
        }
    }
}
