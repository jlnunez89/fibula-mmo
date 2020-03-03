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
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using OpenTibia.Server.Operations.Actions;
    using Serilog;

    /// <summary>
    /// Class that represents a base combat operation.
    /// </summary>
    public class AutoAttackCombatOperation : BaseCombatOperation
    {
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

            this.TargetIdAtScheduleTime = attacker?.AutoAttackTarget?.Id ?? 0;
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
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Gets the id of the target at schedule time.
        /// </summary>
        public uint TargetIdAtScheduleTime { get; }

        /// <summary>
        /// Gets the absolute minimum damage that the combat operation can result in.
        /// </summary>
        public override int MinimumDamage => 0;

        /// <summary>
        /// Gets the absolute maximum damage that the combat operation can result in.
        /// </summary>
        public override int MaximumDamage { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        public override void Execute()
        {
            var distanceBetweenCombatants = (this.Attacker?.Location ?? this.Target.Location) - this.Target.Location;

            var nullAttacker = this.Attacker == null;
            var correctTarget = nullAttacker || this.Attacker?.AutoAttackTarget?.Id == this.TargetIdAtScheduleTime;
            var enoughCredits = nullAttacker || this.Attacker?.AutoAttackCredits >= this.AttackCreditsCost;
            var inChaseMode = nullAttacker ? false : this.Attacker.ChaseMode == ChaseMode.Chase;
            var inRange = nullAttacker ? true : distanceBetweenCombatants.MaxValueIn2D <= this.Attacker.AutoAttackRange && distanceBetweenCombatants.Z == 0;

            var attackPerformed = false;

            if (enoughCredits && correctTarget && inRange)
            {
                attackPerformed |= this.PerformAttack();
            }

            if (!attackPerformed)
            {
                // Update the actual cost if the attack wasn't performed.
                this.ExhaustionCost = TimeSpan.Zero;
            }

            if (!nullAttacker)
            {
                this.Context.Scheduler.CancelAllFor(this.Attacker.Id, typeof(AutoAttackCombatOperation));

                if (!inRange)
                {
                    // Add as the retry action, so that the request gets repeated when the player hits this location.
                    this.Attacker.EnqueueRetryActionWithinRangeToCreature(this.Attacker.AutoAttackRange, this.Target.Id, () => this.AutoAttack(this.Attacker, this.Target));

                    if (inChaseMode)
                    {
                        // Too far away to attack, we need to move closer first.
                        var directions = this.Context.PathFinder.FindBetween(this.Attacker.Location, this.Target.Location, out _, onBehalfOfCreature: this.Attacker, considerAvoidsAsBlock: true);

                        if (directions == null || !directions.Any())
                        {
                            this.SendFailureNotification(OperationMessage.ThereIsNoWay);
                        }
                        else
                        {
                            this.AutoWalk(this.Attacker, directions.ToArray());
                        }
                    }
                }

                // Do we need to continue attacking?
                if (this.Attacker.AutoAttackTarget != null)
                {
                    var normalizedAttackSpeed = TimeSpan.FromMilliseconds((int)Math.Ceiling(ICombatOperation.DefaultCombatRoundTimeInMs / this.Attacker.BaseAttackSpeed));
                    var attackOperation = new AutoAttackCombatOperation(this.Logger, this.Context, this.Attacker, this.Attacker.AutoAttackTarget, exhaustionCost: normalizedAttackSpeed);
                    var attackCooldownRemaining = this.Attacker.CalculateRemainingCooldownTime(attackOperation.ExhaustionType, this.Context.Scheduler.CurrentTime);

                    this.Context.Scheduler.ScheduleEvent(attackOperation, delayTime: attackPerformed ? attackCooldownRemaining : normalizedAttackSpeed);
                }
            }
        }

        private void AutoAttack(ICombatant attacker, ICombatant target)
        {
            if (attacker == null)
            {
                return;
            }

            var attackExhaustionCost = TimeSpan.FromMilliseconds((int)Math.Ceiling(ICombatOperation.DefaultCombatRoundTimeInMs / attacker.BaseAttackSpeed));

            var attackOperation = new AutoAttackCombatOperation(this.Logger, this.Context, attacker, target, attackExhaustionCost);
            var attackCooldownRemaining = attacker.CalculateRemainingCooldownTime(attackOperation.ExhaustionType, this.Context.Scheduler.CurrentTime);

            this.Context.Scheduler.ScheduleEvent(attackOperation, attackCooldownRemaining);
        }

        private bool PerformAttack()
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

            this.Target.ConsumeCredits(CombatCreditType.Defense, 1);

            if (this.Attacker != null)
            {
                this.Target.RecordDamageTaken(this.Attacker.Id, damageToApply);

                this.Attacker.ConsumeCredits(CombatCreditType.Attack, 1);

                if (this.Attacker.Location != this.Target.Location && this.Attacker.Id != this.Target.Id)
                {
                    var directionToTarget = this.Attacker.Location.DirectionTo(this.Target.Location);

                    this.Context.Scheduler.ScheduleEvent(new TurnToDirectionOperation(this.Logger, this.Context, this.Attacker, directionToTarget));
                }

                this.Context.Scheduler.CancelAllFor(this.Attacker.Id, typeof(AutoAttackCombatOperation));
            }

            this.Context.Scheduler.ScheduleEvent(
                new GenericNotification(
                    this.Logger,
                    () => this.Context.ConnectionFinder.PlayersThatCanSee(this.Context.CreatureFinder, this.Target.Location),
                    new GenericNotificationArguments(packetsToSend.ToArray())));

            return true;
        }
    }
}