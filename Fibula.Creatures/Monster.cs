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
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Constants;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Structs;

    /// <summary>
    /// Class that represents all monsters in the game.
    /// </summary>
    public class Monster : CombatantCreature, IMonster
    {
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

            /*
            // make a copy of the type we are based on...
            foreach (var kvp in this.Type.Skills)
            {
                (int defaultLevel, int currentLevel, int maximumLevel, uint targetForNextLevel, uint targetIncreaseFactor, byte increasePerLevel) = kvp.Value;

                this.Skills[kvp.Key] = new MonsterSkill(kvp.Key, defaultLevel, currentLevel, maximumLevel, targetForNextLevel, targetIncreaseFactor, increasePerLevel);
            }

            // Add experience as a skill
            this.Skills[SkillType.Experience] = new MonsterSkill(SkillType.Experience, Math.Max(int.MaxValue, (int)monsterType.Experience), 0, int.MaxValue, 100, 1100, 5);
            */
        }

        /// <summary>
        /// Gets the type of this monster.
        /// </summary>
        public IMonsterType Type { get; }

        /// <summary>
        /// Gets the experience yielded when this monster dies.
        /// </summary>
        public uint Experience => 0; // this.Skills[SkillType.Experience].Level;

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
    }
}
