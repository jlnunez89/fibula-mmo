// -----------------------------------------------------------------
// <copyright file="CombatantCreature.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Data.Entities.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Constants;
    using Fibula.Mechanics.Contracts.Delegates;
    using Fibula.Mechanics.Contracts.Structs;

    /// <summary>
    /// Class that represents all creatures in the game.
    /// </summary>
    public abstract class CombatantCreature : Creature, ICombatant
    {
        /// <summary>
        /// Stores the map of combatants to the damage taken from them for the current combat session.
        /// </summary>
        private readonly ConcurrentDictionary<uint, uint> combatSessionDamageTakenMap;

        /// <summary>
        /// The base of how fast a combatant can earn an attack credit per combat round.
        /// </summary>
        private readonly decimal baseAttackSpeed;

        /// <summary>
        /// The base of how fast a combatant can earn a defense credit per combat round.
        /// </summary>
        private readonly decimal baseDefenseSpeed;

        /// <summary>
        /// Stores the set of combatants currently attacking this combatant for the current combat session.
        /// </summary>
        private readonly ISet<ICombatant> combatSessionAttackedBy;

        /// <summary>
        /// The buff for attack speed.
        /// </summary>
        private decimal attackSpeedBuff;

        /// <summary>
        /// The buff for defense speed.
        /// </summary>
        private decimal defenseSpeedBuff;

        /// <summary>
        /// Initializes a new instance of the <see cref="CombatantCreature"/> class.
        /// </summary>
        /// <param name="name">The name of this creature.</param>
        /// <param name="article">An article for the name of this creature.</param>
        /// <param name="maxHitpoints">The maximum hitpoints of the creature.</param>
        /// <param name="corpse">The corpse of the creature.</param>
        /// <param name="hitpoints">The current hitpoints of the creature.</param>
        /// <param name="baseAttackSpeed">
        /// Optional. The base attack speed for this creature.
        /// Bounded between [<see cref="CombatConstants.MinimumCombatSpeed"/>, <see cref="CombatConstants.MaximumCombatSpeed"/>] inclusive.
        /// Defaults to <see cref="CombatConstants.DefaultAttackSpeed"/>.
        /// </param>
        /// <param name="baseDefenseSpeed">
        /// Optional. The base defense speed for this creature.
        /// Bounded between [<see cref="CombatConstants.MinimumCombatSpeed"/>, <see cref="CombatConstants.MaximumCombatSpeed"/>] inclusive.
        /// Defaults to <see cref="CombatConstants.DefaultDefenseSpeed"/>.
        /// </param>
        protected CombatantCreature(
            string name,
            string article,
            ushort maxHitpoints,
            ushort corpse,
            ushort hitpoints = 0,
            decimal baseAttackSpeed = CombatConstants.DefaultAttackSpeed,
            decimal baseDefenseSpeed = CombatConstants.DefaultDefenseSpeed)
            : base(name, article, maxHitpoints, corpse, hitpoints)
        {
            // Normalize combat speeds.
            this.baseAttackSpeed = Math.Min(CombatConstants.MaximumCombatSpeed, Math.Max(CombatConstants.MinimumCombatSpeed, baseAttackSpeed));
            this.baseDefenseSpeed = Math.Min(CombatConstants.MaximumCombatSpeed, Math.Max(CombatConstants.MinimumCombatSpeed, baseDefenseSpeed));

            this.combatSessionDamageTakenMap = new ConcurrentDictionary<uint, uint>();
            this.combatSessionAttackedBy = new HashSet<ICombatant>();

            this.Skills = new Dictionary<SkillType, ISkill>();

            this.Stats.Add(CreatureStat.AttackPoints, new Stat(CreatureStat.AttackPoints, 1, CombatConstants.DefaultMaximumAttackCredits));
            this.Stats[CreatureStat.AttackPoints].Changed += this.RaiseStatChange;

            this.Stats.Add(CreatureStat.DefensePoints, new Stat(CreatureStat.DefensePoints, 2, CombatConstants.DefaultMaximumDefenseCredits));
            this.Stats[CreatureStat.DefensePoints].Changed += this.RaiseStatChange;

            this.StatChanged += (ICreature creature, IStat statThatChanged, uint previousValue, byte previousPercent) =>
            {
                if (statThatChanged.Type == CreatureStat.HitPoints && statThatChanged.Current == 0)
                {
                    this.Death?.Invoke(this);
                }
            };

            // TODO: need to set
            // combatant.StatChanged = 0;
        }

        /// <summary>
        /// Event to call when the combatant dies.
        /// </summary>
        public event OnDeath Death;

        /// <summary>
        /// Event to call when the attack target changes.
        /// </summary>
        public event OnAttackTargetChanged AttackTargetChanged;

        /// <summary>
        /// Event to call when the follow target changes.
        /// </summary>
        public event OnFollowTargetChanged FollowTargetChanged;

        /// <summary>
        /// Event triggered when a skill of this creature changes.
        /// </summary>
        public event OnCreatureSkillChanged SkillChanged;

        /// <summary>
        /// Gets or sets the target being chased, if any.
        /// </summary>
        public ICreature ChaseTarget { get; protected set; }

        /// <summary>
        /// Gets the current target combatant.
        /// </summary>
        public ICombatant AutoAttackTarget { get; private set; }

        /// <summary>
        /// Gets a metric of how fast a combatant can earn an attack credit per combat round.
        /// </summary>
        public decimal AttackSpeed => this.baseAttackSpeed + this.attackSpeedBuff;

        /// <summary>
        /// Gets a metric of how fast a combatant can earn a defense credit per combat round.
        /// </summary>
        public decimal DefenseSpeed => this.baseDefenseSpeed + this.defenseSpeedBuff;

        /// <summary>
        /// Gets or sets the fight mode selected by this combatant.
        /// </summary>
        public FightMode FightMode { get; set; }

        /// <summary>
        /// Gets or sets the chase mode selected by this combatant.
        /// </summary>
        public ChaseMode ChaseMode { get; set; }

        /// <summary>
        /// Gets the range that the auto attack has.
        /// </summary>
        public abstract byte AutoAttackRange { get; }

        /// <summary>
        /// Gets the distribution of damage taken by any combatant that has attacked this combatant while the current combat is active.
        /// </summary>
        public IEnumerable<(uint CombatantId, uint Damage)> DamageTakenInSession
        {
            get
            {
                return this.combatSessionDamageTakenMap.Select(kvp => (kvp.Key, kvp.Value)).ToList();
            }
        }

        /// <summary>
        /// Gets the collection of combatants currently attacking this combatant.
        /// </summary>
        public IEnumerable<ICombatant> AttackedBy
        {
            get
            {
                return this.combatSessionAttackedBy.ToList();
            }
        }

        /// <summary>
        /// Gets the current skills information for the combatant.
        /// </summary>
        /// <remarks>
        /// The key is a <see cref="SkillType"/>, and the value is a <see cref="ISkill"/>.
        /// </remarks>
        public IDictionary<SkillType, ISkill> Skills { get; }

        /// <summary>
        /// Sets the attack target of this combatant.
        /// </summary>
        /// <param name="otherCombatant">The other target combatant, if any.</param>
        /// <returns>True if the target was actually changed, false otherwise.</returns>
        public bool SetAttackTarget(ICombatant otherCombatant)
        {
            bool targetWasChanged = false;

            if (otherCombatant != this.AutoAttackTarget)
            {
                var oldTarget = this.AutoAttackTarget;

                this.AutoAttackTarget = otherCombatant;

                oldTarget?.UnsetAttackedBy(this);
                otherCombatant?.SetAttackedBy(this);

                if (this.ChaseMode != ChaseMode.Stand)
                {
                    this.SetFollowTarget(otherCombatant);
                }

                this.AttackTargetChanged?.Invoke(this, oldTarget);

                targetWasChanged = true;
            }

            return targetWasChanged;
        }

        /// <summary>
        /// Sets the chasing target of this combatant.
        /// </summary>
        /// <param name="target">The target to chase, if any.</param>
        /// <returns>True if the target was actually changed, false otherwise.</returns>
        public bool SetFollowTarget(ICreature target)
        {
            bool targetWasChanged = false;

            if (target != this.ChaseTarget)
            {
                var oldTarget = this.ChaseTarget;

                this.ChaseTarget = target;

                this.FollowTargetChanged?.Invoke(this, oldTarget);

                targetWasChanged = true;
            }

            return targetWasChanged;
        }

        /// <summary>
        /// Calculates the current percentual value between current and target counts for the given skill.
        /// </summary>
        /// <param name="skillType">The type of skill to calculate for.</param>
        /// <returns>A value between [0, 100] representing the current percentual value.</returns>
        public byte CalculateSkillPercent(SkillType skillType)
        {
            const int LowerBound = 0;
            const int UpperBound = 100;

            if (!this.Skills.ContainsKey(skillType))
            {
                return LowerBound;
            }

            var unadjustedPercent = Math.Max(LowerBound, Math.Min(this.Skills[skillType].Count / this.Skills[skillType].TargetCount, UpperBound));

            return (byte)Math.Floor(unadjustedPercent);
        }

        /// <summary>
        /// Starts tracking another <see cref="ICombatant"/>.
        /// </summary>
        /// <param name="otherCombatant">The other combatant, now in view.</param>
        public abstract void AddToCombatList(ICombatant otherCombatant);

        /// <summary>
        /// Stops tracking another <see cref="ICombatant"/>.
        /// </summary>
        /// <param name="otherCombatant">The other combatant, now in view.</param>
        public abstract void RemoveFromCombatList(ICombatant otherCombatant);

        /// <summary>
        /// Sets this combatant as being attacked by another.
        /// </summary>
        /// <param name="combatant">The combatant attacking this one, if any.</param>
        public void SetAttackedBy(ICombatant combatant)
        {
            if (combatant == null)
            {
                return;
            }

            this.combatSessionAttackedBy.Add(combatant);
        }

        /// <summary>
        /// Unsets this combatant as being attacked by another.
        /// </summary>
        /// <param name="combatant">The combatant no longer attacking this one, if any.</param>
        public void UnsetAttackedBy(ICombatant combatant)
        {
            if (combatant == null)
            {
                return;
            }

            this.combatSessionAttackedBy.Remove(combatant);
        }

        /// <summary>
        /// Adds a value of experience to the combatant, which can be positive or negative.
        /// </summary>
        /// <param name="expToGiveOrTake">The experience value to give or take.</param>
        public void AddExperience(long expToGiveOrTake)
        {
            if (!this.Skills.ContainsKey(SkillType.Experience) || expToGiveOrTake == 0)
            {
                return;
            }

            if (expToGiveOrTake < 0)
            {
                throw new NotSupportedException($"Taking experience is not supported yet. Therefore, {nameof(expToGiveOrTake)} ({expToGiveOrTake}) must not be negative.");
            }

            this.Skills[SkillType.Experience].IncreaseCounter(expToGiveOrTake);
        }

        /// <summary>
        /// Applies damage to the combatant, which is expected to apply reductions and protections.
        /// </summary>
        /// <param name="damageInfo">The information of the damage to make, without reductions.</param>
        /// <param name="fromCombatantId">The combatant from which to track the damage, if any.</param>
        /// <returns>The information about the damage actually done.</returns>
        public DamageInfo ApplyDamage(DamageInfo damageInfo, uint fromCombatantId = 0)
        {
            this.ApplyDamageModifiers(ref damageInfo);

            var currentHitpoints = this.Stats[CreatureStat.HitPoints].Current;

            if (damageInfo.Damage != 0)
            {
                this.Stats[CreatureStat.HitPoints].Decrease(damageInfo.Damage);
            }

            if (damageInfo.Damage > 0)
            {
                damageInfo.Damage = Math.Min(damageInfo.Damage, (int)currentHitpoints);
                damageInfo.Blood = this.BloodType;
                damageInfo.Effect = this.BloodType switch
                {
                    BloodType.Bones => AnimatedEffect.XGray,
                    BloodType.Fire => AnimatedEffect.XBlood,
                    BloodType.Slime => AnimatedEffect.Poison,
                    _ => AnimatedEffect.XBlood,
                };
            }

            if (fromCombatantId > 0)
            {
                this.combatSessionDamageTakenMap.AddOrUpdate(fromCombatantId, (uint)damageInfo.Damage, (key, oldValue) => (uint)(oldValue + damageInfo.Damage));
            }

            return damageInfo;
        }

        /// <summary>
        /// Increases the attack speed of this combatant.
        /// </summary>
        /// <param name="increaseAmount">The amount by which to increase.</param>
        // TODO: this is just for testing purposes and should be removed.
        public void IncreaseAttackSpeed(decimal increaseAmount)
        {
            this.attackSpeedBuff = Math.Min(CombatConstants.MaximumCombatSpeed - this.baseAttackSpeed, this.attackSpeedBuff + increaseAmount);
        }

        /// <summary>
        /// Decreases the attack speed of this combatant.
        /// </summary>
        /// <param name="decreaseAmount">The amount by which to decrease.</param>
        // TODO: this is just for testing purposes and should be removed.
        public void DecreaseAttackSpeed(decimal decreaseAmount)
        {
            this.attackSpeedBuff = Math.Max(0, this.attackSpeedBuff - decreaseAmount);
        }

        /// <summary>
        /// Increases the defense speed of this combatant.
        /// </summary>
        /// <param name="increaseAmount">The amount by which to increase.</param>
        // TODO: this is just for testing purposes and should be removed.
        public void IncreaseDefenseSpeed(decimal increaseAmount)
        {
            this.defenseSpeedBuff = Math.Min(CombatConstants.MaximumCombatSpeed - this.baseDefenseSpeed, this.defenseSpeedBuff + increaseAmount);
        }

        /// <summary>
        /// Decreases the defense speed of this combatant.
        /// </summary>
        /// <param name="decreaseAmount">The amount by which to decrease.</param>
        // TODO: this is just for testing purposes and should be removed.
        public void DecreaseDefenseSpeed(decimal decreaseAmount)
        {
            this.defenseSpeedBuff = Math.Max(0, this.defenseSpeedBuff - decreaseAmount);
        }

        /// <summary>
        /// Raises the <see cref="SkillChanged"/> event for this creature on the given skill.
        /// </summary>
        /// <param name="forSkill">The skill to advance.</param>
        /// <param name="previousLevel">The previous skill level.</param>
        /// <param name="previousPercent">The previous percent of completion to next level.</param>
        /// <param name="countDelta">Optional. The delta in the count for the skill. Not always sent.</param>
        protected void RaiseSkillChange(SkillType forSkill, uint previousLevel, byte previousPercent, long? countDelta = null)
        {
            if (!this.Skills.ContainsKey(forSkill))
            {
                return;
            }

            this.SkillChanged?.Invoke(this, this.Skills[forSkill], previousLevel, previousPercent, countDelta);
        }

        /// <summary>
        /// Applies damage modifiers to the damage information provided.
        /// </summary>
        /// <param name="damageInfo">The damage information.</param>
        protected abstract void ApplyDamageModifiers(ref DamageInfo damageInfo);
    }
}
