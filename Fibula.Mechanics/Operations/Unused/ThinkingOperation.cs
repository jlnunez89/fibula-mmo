// -----------------------------------------------------------------
// <copyright file="ThinkingOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations
{
    using System;
    using System.Linq;

    /// <summary>
    /// Class that represents a thinking operation.
    /// </summary>
    public class ThinkingOperation : Operation
    {
        /// <summary>
        /// A pseudo-random number generator for insternal use.
        /// </summary>
        private readonly Random rng;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThinkingOperation"/> class.
        /// </summary>
        /// <param name="creature">The creature that is thinking.</param>
        /// <param name="thinkingCadence">The cadence in which the operation happens.</param>
        public ThinkingOperation(ICreature creature, TimeSpan thinkingCadence)
            : base(creature.Id)
        {
            creature.ThrowIfNull(nameof(creature));

            this.ExhaustionCost = thinkingCadence;
            this.Creature = creature;

            this.rng = new Random();
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.MentalCombat;

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Gets the creature that is thinking in this operation.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            // This is where the combatant is thinking about what to do during combat.
            // First off: is the creature attacking something?
            if (this.Creature is ICombatant combatant)
            {
                if (combatant.AutoAttackTarget != null)
                {
                    var normalizedAttackSpeed = TimeSpan.FromMilliseconds((int)Math.Ceiling(ICombatOperation.DefaultCombatRoundTimeInMs / combatant.BaseAttackSpeed));
                    var autoAttackOperation = context.OperationFactory.Create(OperationType.AutoAttack, new AutoAttackCombatOperationCreationArguments(combatant.Id, combatant, combatant.AutoAttackTarget, normalizedAttackSpeed));

                    var attackCooldownRemaining = combatant.CalculateRemainingCooldownTime(this.ExhaustionType, context.Scheduler.CurrentTime);

                    context.Scheduler.ScheduleEvent(autoAttackOperation, attackCooldownRemaining);
                }
            }

            // If the creature is a player, let's handle accordingly.
            if (this.Creature is IPlayer player)
            {
                this.ThinkForPlayer(context, player);
            }

            // If the creature is a monster, let's handle accordingly.
            if (this.Creature is IMonster monster)
            {
                this.ThinkForMonster(context, monster);
            }
        }

        private void ThinkForPlayer(IOperationContext context, IPlayer player)
        {

        }

        private void ThinkForMonster(IOperationContext context, IMonster monster)
        {
            var inChaseMode = monster.ChaseMode == ChaseMode.Chase;

            // Monsters should pick or change their current target, usually from their Hostiles list.
            if (monster.AutoAttackTarget == null || (monster.AutoAttackTarget.Location - monster.Location).Z != 0)
            {
                foreach (var hostileId in monster.HostilesInView.OrderBy(_ => this.rng.Next()))
                {
                    if (!(context.CreatureFinder.FindCreatureById(hostileId) is ICombatant targetCombatant) || (monster.Location.Z != targetCombatant.Location.Z))
                    {
                        continue;
                    }

                    monster.SetAttackTarget(targetCombatant);

                    break;
                }
            }
            else
            {
                // TODO: decide when to do abilities/spells, speech, movement.
            }

            if (inChaseMode && monster.ChasingTarget != null)
            {
                // Too far away to attack, we need to move closer first.
                var directions = context.PathFinder.FindBetween(monster.Location, monster.ChasingTarget.Location, out _, onBehalfOfCreature: monster, considerAvoidsAsBlocking: true);

                if (directions != null && directions.Any())
                {
                    var autoWalkOperation = context.OperationFactory.Create(OperationType.AutoWalk, new AutoWalkOperationCreationArguments(this.RequestorId, monster, directions.ToArray()));

                    context.EventRulesApi.ClearAllFor(autoWalkOperation.GetPartitionKey());

                    var movementCooldownRemaining = monster.CalculateRemainingCooldownTime(autoWalkOperation.ExhaustionType, context.Scheduler.CurrentTime);

                    context.Scheduler.ScheduleEvent(autoWalkOperation, movementCooldownRemaining);
                }
            }
        }
    }
}