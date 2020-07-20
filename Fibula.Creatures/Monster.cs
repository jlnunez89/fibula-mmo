// -----------------------------------------------------------------
// <copyright file="Monster.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Constants;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Structs;

    /// <summary>
    /// Class that represents all monsters in the game.
    /// </summary>
    public class Monster : CombatantCreature, IMonster
    {
        /// <summary>
        /// An object used as the lock to semaphore access to <see cref="hostileCombatants"/>.
        /// </summary>
        private readonly object hostileCombatantsLock;

        /// <summary>
        /// Stores the set of combatants that are deemed hostile to this monster.
        /// </summary>
        private readonly ISet<ICombatant> hostileCombatants;

        /// <summary>
        /// Initializes a new instance of the <see cref="Monster"/> class.
        /// </summary>
        /// <param name="monsterType">The type of this monster.</param>
        /// <param name="itemFactory">A reference to the item factory in use, for inventory generation.</param>
        public Monster(IMonsterType monsterType, IItemFactory itemFactory)
            : base(monsterType.Name, monsterType.Article, monsterType.MaxHitPoints, monsterType.MaxManaPoints, monsterType.Corpse)
        {
            this.Type = monsterType;
            this.Outfit = monsterType.Outfit;

            this.BaseSpeed += monsterType.BaseSpeed;

            this.Blood = monsterType.BloodType;
            this.ChaseMode = this.Type.HasFlag(CreatureFlag.DistanceFighting) ? ChaseMode.KeepDistance : ChaseMode.Chase;
            this.FightMode = FightMode.FullAttack;

            this.Inventory = new MonsterInventory(itemFactory, this, monsterType.InventoryComposition);

            this.hostileCombatants = new HashSet<ICombatant>();
            this.hostileCombatantsLock = new object();

            this.InitializeSkills();
        }

        /// <summary>
        /// Gets the type of this monster.
        /// </summary>
        public IMonsterType Type { get; }

        /// <summary>
        /// Gets the experience yielded when this monster dies.
        /// </summary>
        public uint Experience => this.Skills[SkillType.Experience].Level;

        /// <summary>
        /// Gets or sets the inventory for the monster.
        /// </summary>
        public sealed override IInventory Inventory { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this monster can be moved by others.
        /// </summary>
        public override bool CanBeMoved => !this.Type.HasFlag(CreatureFlag.Unpushable);

        /// <summary>
        /// Gets the range that the auto attack has.
        /// </summary>
        public override byte AutoAttackRange => (byte)(this.Type.HasFlag(CreatureFlag.DistanceFighting) ? MonsterConstants.DefaultDistanceFightingAttackRange : MonsterConstants.DefaultMeleeFightingAttackRange);

        /// <summary>
        /// Gets or sets the monster speed.
        /// </summary>
        public override ushort Speed
        {
            get => (ushort)((2 * (this.VariableSpeed + this.BaseSpeed)) + 80);

            protected set => this.BaseSpeed = value;
        }

        /// <summary>
        /// Gets the collection of tracked combatants.
        /// </summary>
        public override IEnumerable<ICombatant> TrackedCombatants
        {
            get
            {
                lock (this.hostileCombatantsLock)
                {
                    return this.hostileCombatants.ToList();
                }
            }
        }

        /// <summary>
        /// Starts tracking another <see cref="ICombatant"/>.
        /// </summary>
        /// <param name="otherCombatant">The other combatant, now in view.</param>
        public override void StartTrackingCombatant(ICombatant otherCombatant)
        {
            if (this == otherCombatant)
            {
                return;
            }

            lock (this.hostileCombatantsLock)
            {
                this.hostileCombatants.Add(otherCombatant);

                if (this.hostileCombatants.Count == 1)
                {
                    this.ChaseMode = this.Type.HasFlag(CreatureFlag.DistanceFighting) ? ChaseMode.KeepDistance : ChaseMode.Chase;
                    this.SetAttackTarget(otherCombatant);
                }
            }
        }

        /// <summary>
        /// Stops tracking another <see cref="ICombatant"/>.
        /// </summary>
        /// <param name="otherCombatant">The other combatant, now in view.</param>
        public override void StopTrackingCombatant(ICombatant otherCombatant)
        {
            if (this == otherCombatant)
            {
                return;
            }

            lock (this.hostileCombatantsLock)
            {
                this.hostileCombatants.Remove(otherCombatant);

                if (otherCombatant == this.AutoAttackTarget)
                {
                    this.SetAttackTarget(this.hostileCombatants.Count > 0 ? this.hostileCombatants.First() : null);
                }
            }
        }

        /// <summary>
        /// Applies damage modifiers to the damage information provided.
        /// </summary>
        /// <param name="damageInfo">The damage information.</param>
        protected override void ApplyDamageModifiers(ref DamageInfo damageInfo)
        {
            var rng = new Random();

            // 75% chance to block it?
            if (this.AutoDefenseCredits > 0 && rng.Next(4) > 0)
            {
                damageInfo.Effect = AnimatedEffect.Puff;
                damageInfo.Damage = 0;
            }

            // 25% chance to hit the armor...
            if (rng.Next(4) == 0)
            {
                damageInfo.Effect = AnimatedEffect.SparkYellow;
                damageInfo.Damage = 0;
            }
        }

        private void InitializeSkills()
        {
            // make a copy of the type we are based on...
            foreach (var kvp in this.Type.Skills)
            {
                (int defaultLevel, int currentLevel, int maximumLevel, uint targetForNextLevel, uint targetIncreaseFactor, byte increasePerLevel) = kvp.Value;

                this.Skills[kvp.Key] = new MonsterSkill(kvp.Key, defaultLevel, currentLevel, maximumLevel, targetForNextLevel, targetIncreaseFactor, increasePerLevel);
                this.Skills[kvp.Key].Advanced += this.RaiseSkillLevelAdvance;
                this.Skills[kvp.Key].PercentChanged += this.RaiseSkillPercentChange;
            }

            // Add experience yield as a skill
            if (!this.Skills.ContainsKey(SkillType.Experience))
            {
                this.Skills[SkillType.Experience] = new MonsterSkill(SkillType.Experience, Math.Min(int.MaxValue, (int)this.Type.BaseExperienceYield), 0, int.MaxValue, 100, 1100, 5);
                this.Skills[SkillType.Experience].Advanced += this.RaiseSkillLevelAdvance;
                this.Skills[SkillType.Experience].PercentChanged += this.RaiseSkillPercentChange;
            }

            if (!this.Skills.ContainsKey(SkillType.Shield))
            {
                this.Skills[SkillType.Shield] = new Skill(SkillType.Shield, 10, rate: 1.1, 10, 10, 150);
                this.Skills[SkillType.Shield].Advanced += this.RaiseSkillLevelAdvance;
                this.Skills[SkillType.Shield].PercentChanged += this.RaiseSkillPercentChange;
            }

            if (!this.Skills.ContainsKey(SkillType.NoWeapon))
            {
                this.Skills[SkillType.NoWeapon] = new Skill(SkillType.NoWeapon, 1, rate: 1.0, 100, 1, 0);
                this.Skills[SkillType.NoWeapon].Advanced += this.RaiseSkillLevelAdvance;
                this.Skills[SkillType.NoWeapon].PercentChanged += this.RaiseSkillPercentChange;
            }
        }
    }
}
