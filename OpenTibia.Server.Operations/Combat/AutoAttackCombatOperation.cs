// -----------------------------------------------------------------
// <copyright file="AutoAttackCombatOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Combat
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
    using OpenTibia.Server.Operations.Conditions;
    using OpenTibia.Server.Operations.Movements;
    using OpenTibia.Server.Operations.Notifications;
    using OpenTibia.Server.Operations.Notifications.Arguments;
    using Serilog;

    /// <summary>
    /// Class that represents a base combat operation.
    /// </summary>
    public class AutoAttackCombatOperation : BaseCombatOperation
    {
        private const int DefaultAttackRangeDistance = 1;

        private const decimal MaximumCombatSpeed = 5.0m;

        private const decimal MinimumCombatSpeed = 0.2m;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoAttackCombatOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The context of this operation.</param>
        /// <param name="attacker">The combatant that is attacking.</param>
        /// <param name="target">The combatant that is the target.</param>
        /// <param name="exhaustionCost">Optional. The exhaustion cost of this operation.</param>
        public AutoAttackCombatOperation(ILogger logger, IOperationContext context, ICombatant attacker, ICombatant target, TimeSpan exhaustionCost)
            : base(logger, context, target, attacker)
        {
            attacker.ThrowIfNull(nameof(attacker));

            this.ExhaustionCost = exhaustionCost;

            this.Conditions.Add(new AttackingCorrectTargetEventCondition(attacker, attacker?.AutoAttackTarget?.Id ?? 0));
            this.Conditions.Add(new LocationsAreDistantByEventCondition(() => attacker?.Location ?? target.Location, () => target.Location, attacker?.AutoAttackRange ?? DefaultAttackRangeDistance, sameFloorOnly: true));

            // Add this next attack action to both scenarios.
            this.ActionsOnFail.Add(() =>
            {
                // Do we need to continue attacking?
                if (this.Attacker.AutoAttackTarget != null)
                {
                    this.AutoAttack(this.Attacker, this.Attacker.AutoAttackTarget);
                }
            });

            this.ActionsOnPass.Add(() =>
            {
                this.PerformAttack();

                // Do we need to continue attacking?
                if (this.Attacker.AutoAttackTarget != null)
                {
                    this.AutoAttack(this.Attacker, this.Attacker.AutoAttackTarget);
                }
            });
        }

        /// <summary>
        /// Gets the combat operation's attack type.
        /// </summary>
        public override AttackType AttackType => AttackType.Physical;

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.PhysicalCombat;

        /// <summary>
        /// Gets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; }

        /// <summary>
        /// Gets the absolute minimum damage that the combat operation can result in.
        /// </summary>
        public override int MinimumDamage => 0;

        /// <summary>
        /// Gets the absolute maximum damage that the combat operation can result in.
        /// </summary>
        public override int MaximumDamage { get; }

        private void AutoAttack(ICombatant attacker, ICombatant target)
        {
            var locationDiff = attacker.Location - target.Location;

            if (locationDiff.MaxValueIn2D > attacker.AutoAttackRange)
            {
                if (attacker.ChaseMode == ChaseMode.Chase)
                {
                    // Too far away to attack, we need to move closer first.
                    var directions = this.Context.PathFinder.FindBetween(attacker.Location, target.Location, out _, onBehalfOfCreature: attacker, considerAvoidsAsBlock: true);

                    if (directions != null && directions.Any())
                    {
                        this.AutoWalk(attacker, directions.ToArray());
                    }
                }

                // We basically add this request as the retry action, so that the request gets repeated when the player hits this location.
                attacker.EnqueueRetryActionWithinRangeToCreature(attacker.AutoAttackRange, target.Id, () => this.AutoAttack(attacker, target));

                return;
            }

            var normalizedAttackSpeed = Math.Min(MaximumCombatSpeed, Math.Max(MinimumCombatSpeed, attacker.BaseAttackSpeed));
            var attackCost = TimeSpan.FromMilliseconds((int)Math.Ceiling(ICombatOperation.DefaultCombatRoundTimeInMs / normalizedAttackSpeed));

            IEvent attackOperation = new AutoAttackCombatOperation(this.Logger, this.Context, attacker, target, attackCost);

            var cooldownRemaining = attacker.CalculateRemainingCooldownTime(ExhaustionType.PhysicalCombat, this.Context.Scheduler.CurrentTime);

            this.Context.Scheduler.ScheduleEvent(attackOperation, this.Context.Scheduler.CurrentTime + cooldownRemaining);
        }

        private void PerformAttack()
        {
            int CalculateInflictedDamage(out bool armorBlock, out bool wasShielded)
            {
                armorBlock = false;
                wasShielded = false;

                if (this.Target.AutoDefenseCredits > 0)
                {
                    wasShielded = true;

                    return 0;
                }

                var rng = new Random();

                // Coin toss 25% chance to hit the armor...
                if (rng.Next(4) > 0)
                {
                    return rng.Next(10) + 1;
                }

                armorBlock = true;

                return 0;
            }

            AnimatedEffect GetEffect(int damage, bool wasBlockedByArmor)
            {
                if (damage < 0)
                {
                    return AnimatedEffect.GlitterBlue;
                }
                else if (damage == 0)
                {
                    return wasBlockedByArmor ? AnimatedEffect.SparkYellow : AnimatedEffect.Puff;
                }

                switch (this.Target.Blood)
                {
                    default:
                    case BloodType.Fire:
                    case BloodType.Blood:
                        return AnimatedEffect.XBlood;
                    case BloodType.Bones:
                        return AnimatedEffect.XGray;
                    case BloodType.Slime:
                        return AnimatedEffect.Poison;
                }
            }

            TextColor GetTextColor(int damage)
            {
                if (damage < 0)
                {
                    return TextColor.Blue;
                }

                switch (this.Target.Blood)
                {
                    default:
                    case BloodType.Blood:
                        return TextColor.Red;
                    case BloodType.Bones:
                        return TextColor.LightGrey;
                    case BloodType.Fire:
                        return TextColor.Orange;
                    case BloodType.Slime:
                        return TextColor.Green;
                }
            }

            var damageToApply = CalculateInflictedDamage(out bool wasArmorBlock, out bool wasShielded);

            var packetsToSend = new List<IOutgoingPacket>()
            {
                new MagicEffectPacket(this.Target.Location, GetEffect(damageToApply, wasArmorBlock)),
            };

            if (damageToApply != 0)
            {
                packetsToSend.Add(new AnimatedTextPacket(this.Target.Location, GetTextColor(damageToApply), Math.Abs(damageToApply).ToString()));
            }

            this.Context.Scheduler.ImmediateEvent(
                new GenericNotification(
                    this.Logger,
                    () => this.Context.ConnectionFinder.PlayersThatCanSee(this.Context.CreatureFinder, this.Target.Location),
                    new GenericNotificationArguments(packetsToSend.ToArray())));

            this.Target.ConsumeCredits(CombatCreditType.Defense, 1);

            if (this.Attacker != null)
            {
                this.Target.RecordDamageTaken(this.Attacker.Id, damageToApply);

                this.Attacker.ConsumeCredits(CombatCreditType.Attack, 1);

                this.Attacker.AddExhaustion(ExhaustionType.PhysicalCombat, this.Context.Scheduler.CurrentTime, this.ExhaustionCost);
            }
        }

        /// <summary>
        /// Schedules autowalking by a creature in the directions supplied.
        /// </summary>
        /// <param name="creature">The creature walking.</param>
        /// <param name="directions">The directions to follow.</param>
        /// <param name="stepIndex">Optional. The index of the current direction.</param>
        private void AutoWalk(ICreature creature, Direction[] directions, int stepIndex = 0)
        {
            if (directions.Length == 0 || stepIndex >= directions.Length)
            {
                return;
            }

            // A new request overrides and cancels any movement waiting to be retried.
            this.Context.Scheduler.CancelAllFor(creature.Id, typeof(IMovementOperation));

            var nextLocation = creature.Location.LocationAt(directions[stepIndex]);

            // TODO: revise exhaustion logic.
            TimeSpan movementDelay = creature.CalculateRemainingCooldownTime(ExhaustionType.Movement, this.Context.Scheduler.CurrentTime);

            this.Context.Scheduler.ScheduleEvent(
                new MapToMapMovementOperation(
                    this.Logger,
                    this.Context,
                    creature.Id,
                    creature,
                    creature.Location,
                    nextLocation),
                this.Context.Scheduler.CurrentTime + movementDelay);

            if (directions.Length > 1)
            {
                // Add this request as the retry action, so that the request gets repeated when the player hits this location.
                creature.EnqueueRetryActionAtLocation(nextLocation, () => this.AutoWalk(creature, directions, stepIndex + 1));
            }
        }
    }
}