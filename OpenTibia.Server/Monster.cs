// -----------------------------------------------------------------
// <copyright file="Monster.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Monsters
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents all monsters in the game.
    /// </summary>
    public class Monster : CombatantCreature, IMonster
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Monster"/> class.
        /// </summary>
        /// <param name="monsterType">The type of this monster.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        public Monster(IMonsterType monsterType, IItemFactory itemFactory)
            : base(monsterType.Name, monsterType.Article, monsterType.MaxHitPoints, monsterType.MaxManaPoints, monsterType.Corpse)
        {
            this.Type = monsterType;
            this.Speed += monsterType.Speed;
            this.Outfit = monsterType.Outfit;

            this.Blood = monsterType.Blood;
            this.ChaseMode = this.Type.Flags.HasFlag((uint)CreatureFlag.DistanceFighting) ? ChaseMode.KeepDistance : ChaseMode.Chase;
            this.FightMode = FightMode.FullAttack;

            this.Inventory = new MonsterInventory(itemFactory, this, monsterType.InventoryComposition);

            // make a copy of the type we are based on...
            foreach (var kvp in this.Type.Skills)
            {
                (int defaultLevel, int currentLevel, int maximumLevel, uint targetForNextLevel, uint targetIncreaseFactor, byte increasePerLevel) = kvp.Value;

                this.Skills[kvp.Key] = new MonsterSkill(kvp.Key, defaultLevel, currentLevel, maximumLevel, targetForNextLevel, targetIncreaseFactor, increasePerLevel);
            }

            // Add experience as a skill
            this.Skills[SkillType.Experience] = new MonsterSkill(SkillType.Experience, Math.Max(int.MaxValue, (int)monsterType.Experience), 0, int.MaxValue, 100, 1100, 5);
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
        public override bool CanBeMoved => !this.Type.Flags.HasFlag((uint)CreatureFlag.Unpushable);

        /// <summary>
        /// Gets the range that the auto attack has.
        /// </summary>
        public override byte AutoAttackRange => (byte)(this.Type.Flags.HasFlag((uint)CreatureFlag.DistanceFighting) ? IMonster.DefaultDistanceFightingAttackRange : IMonster.DefaultMeleeFightingAttackRange);

        /// <summary>
        /// Gets the attack power of this combatant.
        /// </summary>
        public override ushort AttackPower => (ushort)(this.Type.Attack + this.Inventory.EquipmentAttackPower);

        /// <summary>
        /// Gets the defense power of this combatant.
        /// </summary>
        public override ushort DefensePower => (ushort)(this.Type.Defense + this.Inventory.EquipmentDefensePower);

        /// <summary>
        /// Gets the armor rating of this combatant.
        /// </summary>
        public override ushort ArmorRating => (ushort)(this.Type.Armor + this.Inventory.EquipmentArmorRating);

        /// <summary>
        /// Gets or sets the chase mode selected by this combatant.
        /// </summary>
        public override ChaseMode ChaseMode { get; set; }

        /// <summary>
        /// Gets or sets the fight mode selected by this combatant.
        /// </summary>
        public override FightMode FightMode { get; set; }

        /// <summary>
        /// Sets a <see cref="ICombatant"/> now in view for this combatant.
        /// </summary>
        /// <param name="otherCombatant">The other combatant, now in view.</param>
        public override void CombatantNowInView(ICombatant otherCombatant)
        {
            otherCombatant.ThrowIfNull(nameof(otherCombatant));

            var totalInViewBefore = this.HostilesInView.Count + this.NeutralsInView.Count + this.FriendlyInView.Count;

            if (otherCombatant is Monster otherMonster)
            {
                // TODO: consider monsters that are not friendly.
                this.FriendlyInView.Add(otherMonster.Id);
            }
            else if (otherCombatant is IPlayer player)
            {
                this.HostilesInView.Add(player.Id);
            }
            else
            {
                this.NeutralsInView.Add(otherCombatant.Id);
            }

            if (totalInViewBefore == 0)
            {
                this.InvokeCombatStarted();
            }
        }

        /// <summary>
        /// Sets a <see cref="ICombatant"/> as no longer in view for this combatant.
        /// </summary>
        /// <param name="otherCombatant">The other combatant, now in view.</param>
        public override void CombatantNoLongerInView(ICombatant otherCombatant)
        {
            otherCombatant.ThrowIfNull(nameof(otherCombatant));

            if (this.AutoAttackTarget == otherCombatant)
            {
                this.SetAttackTarget(null);
            }

            this.FriendlyInView.Remove(otherCombatant.Id);
            this.HostilesInView.Remove(otherCombatant.Id);
            this.NeutralsInView.Remove(otherCombatant.Id);

            var totalInViewAfter = this.HostilesInView.Count + this.NeutralsInView.Count + this.FriendlyInView.Count;

            if (totalInViewAfter == 0)
            {
                this.InvokeCombatEnded();
            }
        }
    }
}
