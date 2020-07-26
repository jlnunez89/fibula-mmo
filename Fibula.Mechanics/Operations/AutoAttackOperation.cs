// -----------------------------------------------------------------
// <copyright file="AutoAttackOperation.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Data.Entities.Contracts.Enumerations;
    using Fibula.Map.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Combat.Enumerations;
    using Fibula.Mechanics.Contracts.Constants;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Structs;
    using Fibula.Mechanics.Notifications;

    /// <summary>
    /// Class that represents an auto attack operation.
    /// </summary>
    public class AutoAttackOperation : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoAttackOperation"/> class.
        /// </summary>
        /// <param name="attacker">The combatant that is attacking.</param>
        /// <param name="target">The combatant that is the target.</param>
        /// <param name="exhaustionCost">Optional. The exhaustion cost of this operation.</param>
        public AutoAttackOperation(ICombatant attacker, ICombatant target, TimeSpan exhaustionCost)
            : base(attacker?.Id ?? 0)
        {
            attacker.ThrowIfNull(nameof(attacker));
            target.ThrowIfNull(nameof(target));

            this.Target = target;
            this.Attacker = attacker;

            this.ExhaustionCost = exhaustionCost;
            this.TargetIdAtScheduleTime = attacker?.AutoAttackTarget?.Id ?? 0;
        }

        /// <summary>
        /// Gets the combatant that is attacking on this operation.
        /// </summary>
        public ICombatant Attacker { get; }

        /// <summary>
        /// Gets the combatant that is the target on this operation.
        /// </summary>
        public ICombatant Target { get; }

        ///// <summary>
        ///// Gets the combat operation's attack type.
        ///// </summary>
        // public override AttackType AttackType => AttackType.Physical;

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
        public int MinimumDamage => 0;

        /// <summary>
        /// Gets the absolute maximum damage that the combat operation can result in.
        /// </summary>
        public int MaximumDamage { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            // We should stop any pending attack operation before carrying this one out.
            if (this.Attacker.TryRetrieveTrackedOperation(nameof(AutoAttackOperation), out IOperation attackersAtkOp) && attackersAtkOp != this)
            {
                // Cancel it first, and remove it.
                if (attackersAtkOp.Cancel())
                {
                    this.Attacker.StopTrackingEvent(attackersAtkOp);
                }
            }

            var distanceBetweenCombatants = (this.Attacker?.Location ?? this.Target.Location) - this.Target.Location;

            // Pre-checks.
            var nullAttacker = this.Attacker == null;
            var isTargetAlreadyDead = this.Target.IsDead;
            var isCorrectTarget = nullAttacker || this.Attacker?.AutoAttackTarget?.Id == this.TargetIdAtScheduleTime;
            var enoughCredits = nullAttacker || this.Attacker?.AutoAttackCredits >= 1;
            var inRange = nullAttacker || (distanceBetweenCombatants.MaxValueIn2D <= this.Attacker.AutoAttackRange && distanceBetweenCombatants.Z == 0);
            var atIdealDistance = nullAttacker || distanceBetweenCombatants.MaxValueIn2D == this.Attacker.AutoAttackRange;
            var canAttackFromThere = nullAttacker || (inRange && context.Map.CanThrowBetweenLocations(this.Attacker.Location, this.Target.Location));

            var attackPerformed = false;

            try
            {
                if (!isCorrectTarget || isTargetAlreadyDead)
                {
                    // We're not attacking the correct target, so stop right here.
                    return;
                }

                if (!canAttackFromThere)
                {
                    if (!nullAttacker)
                    {
                        // Set the pending attack operation as this operation.
                        this.Attacker.StartTrackingEvent(this);

                        // And set this operation to repeat after some time (we chose it to be 2x the normalized attack speed), so that it can actually
                        // be expedited (or else it's just processed as usual).
                        this.RepeatAfter = TimeSpan.FromMilliseconds((int)Math.Ceiling(CombatConstants.DefaultCombatRoundTimeInMs / this.Attacker.AttackSpeed * 2));

                        if (this.Attacker.ChaseMode != ChaseMode.Stand && this.Attacker.ChaseTarget != null && this.Attacker.WalkPlan.State != WalkPlanState.OnTrack)
                        {
                            context.GameApi.ResetCreatureDynamicWalkPlan(this.Attacker, this.Attacker.ChaseTarget, targetDistance: this.Attacker.AutoAttackRange);
                        }
                    }

                    return;
                }

                if (!atIdealDistance)
                {
                    // While we can actually attack, we may want to move away or closer.
                    if (this.Attacker.ChaseMode == ChaseMode.KeepDistance && this.Attacker.ChaseTarget != null && this.Attacker.WalkPlan.State != WalkPlanState.OnTrack)
                    {
                        context.GameApi.ResetCreatureDynamicWalkPlan(this.Attacker, this.Attacker.ChaseTarget, targetDistance: this.Attacker.AutoAttackRange);
                    }
                }

                if (!enoughCredits)
                {
                    return;
                }

                this.PerformAttack(context);

                attackPerformed = true;
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

        /// <summary>
        /// Performs the auto attack from the <see cref="Attacker"/> to it's <see cref="Target"/>.
        /// </summary>
        /// <param name="context">The context of the operation.</param>
        private void PerformAttack(IOperationContext context)
        {
            var rng = new Random();

            // Calculate the damage to inflict without any protections and reductions,
            // i.e. the amount of damage that the attacker can generate as it is.
            var attackPower = rng.Next(10) + 1;

            var damageToApplyInfo = new DamageInfo(attackPower);
            var damageDoneInfo = this.Target.ApplyDamage(damageToApplyInfo, this.Attacker?.Id ?? 0);
            var distanceOfAttack = (this.Target.Location - (this.Attacker?.Location ?? this.Target.Location)).MaxValueIn2D;

            var packetsToSend = new List<IOutboundPacket>
            {
                new MagicEffectPacket(this.Target.Location, damageDoneInfo.Effect),
            };

            if (damageDoneInfo.Damage != 0)
            {
                TextColor damageTextColor = damageDoneInfo.Blood switch
                {
                    BloodType.Bones => TextColor.LightGrey,
                    BloodType.Fire => TextColor.Orange,
                    BloodType.Slime => TextColor.Green,
                    _ => TextColor.Red,
                };

                if (damageDoneInfo.Damage < 0)
                {
                    damageTextColor = TextColor.LightBlue;
                }

                packetsToSend.Add(new AnimatedTextPacket(this.Target.Location, damageTextColor, Math.Abs(damageDoneInfo.Damage).ToString()));
            }

            if (distanceOfAttack > 1)
            {
                // TODO: actual projectile value.
                packetsToSend.Add(new ProjectilePacket(this.Attacker.Location, this.Target.Location, ProjectileType.Bolt));
            }

            this.Target.ConsumeCredits(CombatCreditType.Defense, 1);

            this.Target.Skills[SkillType.Shield].IncreaseCounter(1);

            if (damageDoneInfo.ApplyBloodToEnvironment)
            {
                context.GameApi.CreateItemAtLocation(this.Target.Location, context.PredefinedItemSet.FindSplatterForBloodType(this.Target.BloodType));
            }

            // Normalize the attacker's defense speed based on the global round time and round that up.
            context.Scheduler.ScheduleEvent(
                new RestoreCombatCreditOperation(this.Target, CombatCreditType.Defense),
                TimeSpan.FromMilliseconds((int)Math.Round(CombatConstants.DefaultCombatRoundTimeInMs / this.Target.DefenseSpeed)));

            if (this.Attacker != null)
            {
                // this.Target.RecordDamageTaken(this.Attacker.Id, damageToApply);
                this.Attacker.ConsumeCredits(CombatCreditType.Attack, 1);

                // TODO: increase the actual skill.
                this.Attacker.Skills[SkillType.NoWeapon].IncreaseCounter(1);

                // Normalize the attacker's attack speed based on the global round time and round that up.
                context.Scheduler.ScheduleEvent(
                    new RestoreCombatCreditOperation(this.Attacker, CombatCreditType.Attack),
                    TimeSpan.FromMilliseconds((int)Math.Round(CombatConstants.DefaultCombatRoundTimeInMs / this.Attacker.AttackSpeed)));

                if (this.Attacker.Location != this.Target.Location && this.Attacker.Id != this.Target.Id)
                {
                    var directionToTarget = this.Attacker.Location.DirectionTo(this.Target.Location);
                    var turnToDirOp = new TurnToDirectionOperation(this.Attacker, directionToTarget);

                    context.Scheduler.ScheduleEvent(turnToDirOp);
                }
            }

            this.SendNotification(context, new GenericNotification(() => context.Map.PlayersThatCanSee(this.Target.Location), packetsToSend.ToArray()));

            if (this.Target is IPlayer targetPlayer)
            {
                var squarePacket = new SquarePacket(this.Attacker.Id, SquareColor.Black);

                this.SendNotification(context, new GenericNotification(() => targetPlayer.YieldSingleItem(), squarePacket));
            }
        }
    }
}
