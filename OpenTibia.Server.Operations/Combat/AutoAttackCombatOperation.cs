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
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using OpenTibia.Server.Operations.Arguments;

    /// <summary>
    /// Class that represents an auto attack combat operation.
    /// </summary>
    public class AutoAttackCombatOperation : BaseCombatOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoAttackCombatOperation"/> class.
        /// </summary>
        /// <param name="attacker">The combatant that is attacking.</param>
        /// <param name="target">The combatant that is the target.</param>
        /// <param name="exhaustionCost">Optional. The exhaustion cost of this operation.</param>
        public AutoAttackCombatOperation(ICombatant attacker, ICombatant target, TimeSpan exhaustionCost)
            : base(target, attacker)
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
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            var distanceBetweenCombatants = (this.Attacker?.Location ?? this.Target.Location) - this.Target.Location;

            // Pre-checks.
            var nullAttacker = this.Attacker == null;
            var isCorrectTarget = nullAttacker || this.Attacker?.AutoAttackTarget?.Id == this.TargetIdAtScheduleTime;
            var enoughCredits = nullAttacker || this.Attacker?.AutoAttackCredits >= this.AttackCreditsCost;
            var inRange = nullAttacker ? true : distanceBetweenCombatants.MaxValueIn2D <= this.Attacker.AutoAttackRange && distanceBetweenCombatants.Z == 0;

            var attackPerformed = false;

            try
            {
                if (!isCorrectTarget)
                {
                    // We're not attacking the correct target, so stop right here.
                    return;
                }

                var rulesPartitionKey = $"{nameof(AutoAttackCombatOperation)}:{this.Attacker.Id}";

                if (inRange)
                {
                    context.EventRulesApi.ClearAllFor(rulesPartitionKey);

                    attackPerformed = enoughCredits && this.PerformAttack(context);
                }
                else if (!nullAttacker)
                {
                    // Setup as a movement rule, so that it gets expedited when the combatant is in range from it's target.
                    var conditionsForExpedition = new Func<IEventRuleContext, bool>[]
                    {
                        (context) =>
                        {
                            if (!(context.Arguments is MovementEventRuleArguments movementEventRuleArguments) ||
                                !(movementEventRuleArguments.ThingMoving is ICombatant attacker) ||
                                !(attacker.AutoAttackTarget is ICombatant target))
                            {
                                return false;
                            }

                            return (target.Location - attacker.Location).MaxValueIn2D <= attacker.AutoAttackRange;
                        },
                    };

                    context.EventRulesApi.SetupRule(new ExpediteOperationMovementEventRule(context.Logger, this, conditionsForExpedition, 1), rulesPartitionKey);
                }

                if (nullAttacker)
                {
                    // Stop here if we don't have an attacker, as we don't need to repeat this operation.
                    return;
                }

                // Do we need to continue attacking?
                if (this.Attacker.AutoAttackTarget != null)
                {
                    var normalizedAttackSpeed = TimeSpan.FromMilliseconds((int)Math.Ceiling(ICombatOperation.DefaultCombatRoundTimeInMs / this.Attacker.BaseAttackSpeed));
                    var attackCooldownRemaining = this.Attacker.CalculateRemainingCooldownTime(this.ExhaustionType, context.Scheduler.CurrentTime);

                    // Update exhaustion cost in case the creature's attack speed changed.
                    this.ExhaustionCost = attackPerformed ? normalizedAttackSpeed : TimeSpan.Zero;

                    // Setting repeat members to repeat this operation.
                    this.Repeat = isCorrectTarget;
                    this.RepeatDelay = attackPerformed ? attackCooldownRemaining : normalizedAttackSpeed;
                }
            }
            finally
            {
                if (!attackPerformed)
                {
                    // Update the actual cost if the attack wasn't performed.
                    this.ExhaustionCost = TimeSpan.Zero;
                }
            }
        }

        private bool PerformAttack(IOperationContext context)
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

                    context.Scheduler.ScheduleEvent(context.OperationFactory.Create(OperationType.Turn, new TurnToDirectionOperationCreationArguments(this.Attacker, directionToTarget)));
                }
            }

            context.Scheduler.ScheduleEvent(
                new GenericNotification(
                    () => context.ConnectionFinder.PlayersThatCanSee(context.CreatureFinder, this.Target.Location),
                    new GenericNotificationArguments(packetsToSend.ToArray())));

            return true;
        }
    }
}