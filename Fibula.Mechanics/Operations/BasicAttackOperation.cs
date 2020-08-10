// -----------------------------------------------------------------
// <copyright file="BasicAttackOperation.cs" company="2Dudes">
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
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Data.Entities.Contracts.Enumerations;
    using Fibula.Map.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Constants;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Structs;
    using Fibula.Mechanics.Notifications;

    /// <summary>
    /// Class that represents the basic attack operation.
    /// </summary>
    public class BasicAttackOperation : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAttackOperation"/> class.
        /// </summary>
        /// <param name="attacker">The combatant that is attacking.</param>
        /// <param name="target">The combatant that is the target.</param>
        /// <param name="exhaustionCost">Optional. The exhaustion cost of this operation.</param>
        public BasicAttackOperation(ICombatant attacker, ICombatant target, TimeSpan exhaustionCost)
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
            if (this.Attacker.TryRetrieveTrackedOperation(nameof(BasicAttackOperation), out IOperation attackersAtkOp) && attackersAtkOp != this)
            {
                // Cancel it first, and remove it.
                attackersAtkOp.Cancel();
            }

            var distanceBetweenCombatants = (this.Attacker?.Location ?? this.Target.Location) - this.Target.Location;

            // Pre-checks.
            var nullAttacker = this.Attacker == null;
            var isTargetAlreadyDead = this.Target.IsDead;
            var isCorrectTarget = nullAttacker || this.Attacker?.AutoAttackTarget?.Id == this.TargetIdAtScheduleTime;
            var enoughCredits = nullAttacker || this.Attacker?.Stats[CreatureStat.AttackPoints].Current > 0;
            var inRange = nullAttacker || (distanceBetweenCombatants.MaxValueIn2D <= this.Attacker.AutoAttackRange && distanceBetweenCombatants.Z == 0);
            var atIdealDistance = nullAttacker || distanceBetweenCombatants.MaxValueIn2D == this.Attacker.AutoAttackRange;
            var attackerIsMonster = !nullAttacker && this.Attacker is IMonster;
            var canAttackFromThere = nullAttacker || (inRange && context.Map.CanThrowBetweenLocations(this.Attacker.Location, this.Target.Location));

            var attackPerformed = false;

            try
            {
                if (!isCorrectTarget || isTargetAlreadyDead)
                {
                    // We're not attacking the correct target or it's already dead, so stop right here.
                    return;
                }

                if (!canAttackFromThere)
                {
                    if (!nullAttacker)
                    {
                        // Set the pending attack operation as this operation.
                        this.Attacker.StartTrackingEvent(this);

                        // And set this operation to repeat after some time, so that it can actually be expedited (or else it's just processed as usual).
                        this.RepeatAfter = TimeSpan.FromMilliseconds((int)Math.Ceiling(CombatConstants.DefaultCombatRoundTimeInMs / this.Attacker.AttackSpeed));
                    }

                    return;
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
            var attackPower = 1 + rng.Next(this.Attacker == null ? 10 : (int)this.Attacker.Skills[SkillType.NoWeapon].Level);

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

            if (this.Target.Stats[CreatureStat.DefensePoints].Decrease(1))
            {
                context.Scheduler.ScheduleEvent(
                    new StatRegenerationOperation(this.Target, CreatureStat.DefensePoints),
                    TimeSpan.FromMilliseconds((int)Math.Round(CombatConstants.DefaultCombatRoundTimeInMs / this.Target.DefenseSpeed)));
            }

            this.Target.Skills[SkillType.Shield].IncreaseCounter(1);

            if (damageDoneInfo.ApplyBloodToEnvironment)
            {
                context.GameApi.CreateItemAtLocation(this.Target.Location, context.PredefinedItemSet.FindSplatterForBloodType(this.Target.BloodType));
            }

            if (this.Attacker != null)
            {
                // TODO: increase the actual skill.
                this.Attacker.Skills[SkillType.NoWeapon].IncreaseCounter(1);

                if (this.Attacker.Stats[CreatureStat.AttackPoints].Decrease(1))
                {
                    context.Scheduler.ScheduleEvent(
                        new StatRegenerationOperation(this.Attacker, CreatureStat.AttackPoints),
                        TimeSpan.FromMilliseconds((int)Math.Round(CombatConstants.DefaultCombatRoundTimeInMs / this.Attacker.AttackSpeed)));
                }

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
