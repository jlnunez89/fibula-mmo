// -----------------------------------------------------------------
// <copyright file="ThinkingOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations
{
    using System;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Operations.Arguments;

    /// <summary>
    /// Class that represents a thinking operation.
    /// </summary>
    public class ThinkingOperation : Operation
    {
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
            if (this.Creature is IMonster monster)
            {
                Random rng = new Random();

                var inChaseMode = monster.ChaseMode == ChaseMode.Chase;

                // Monsters should pick or change their current target, usually from their Hostiles list.
                if (monster.AutoAttackTarget == null || (monster.AutoAttackTarget.Location - monster.Location).Z != 0)
                {
                    foreach (var hostileId in monster.HostilesInView.OrderBy(_ => rng.Next()))
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
                    var directions = context.PathFinder.FindBetween(monster.Location, monster.ChasingTarget.Location, out _, onBehalfOfCreature: monster, considerAvoidsAsBlock: true);

                    if (directions != null && directions.Any())
                    {
                        var autoWalkOperation = context.OperationFactory.Create(OperationType.AutoWalk, new AutoWalkOperationCreationArguments(this.RequestorId, monster, directions.ToArray()));
                        var movementCooldownRemaining = monster.CalculateRemainingCooldownTime(autoWalkOperation.ExhaustionType, context.Scheduler.CurrentTime);

                        context.Scheduler.ScheduleEvent(autoWalkOperation, movementCooldownRemaining);
                    }
                }

                if (monster.HostilesInView.Any() || monster.NeutralsInView.Any())
                {
                    // Set retry members to force retry of this operation.
                    this.Repeat = true;
                    this.RepeatDelay = monster.CalculateRemainingCooldownTime(this.ExhaustionType, context.Scheduler.CurrentTime);
                }
            }
        }
    }
}