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
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents all monsters in the game.
    /// </summary>
    public class Monster : CombatantCreature, ICombatant
    {
        private const int MeleeFightingMonsterAttackRange = 1;

        private const int DistanceFightingMonsterAttackRange = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="Monster"/> class.
        /// </summary>
        /// <param name="monsterType">The type of this monster.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        public Monster(IMonsterType monsterType, IItemFactory itemFactory)
            : base(monsterType.Name, monsterType.Article, monsterType.MaxHitPoints, monsterType.MaxManaPoints, monsterType.Corpse)
        {
            this.Type = monsterType;
            this.Experience = monsterType.Experience;
            this.Speed += monsterType.Speed;
            this.Outfit = monsterType.Outfit;

            this.Blood = monsterType.Blood;
            this.ChaseMode = this.Type.Flags.HasFlag((uint)CreatureFlag.DistanceFighting) ? ChaseMode.KeepDistance : ChaseMode.Chase;

            this.Inventory = new MonsterInventory(itemFactory, this, monsterType.InventoryComposition);

            // make a copy of the type we are based on...
            foreach (var kvp in this.Type.Skills)
            {
                (int defaultLevel, int currentLevel, int maximumLevel, uint targetForNextLevel, uint targetIncreaseFactor, byte increasePerLevel) = kvp.Value;

                this.Skills[kvp.Key] = new MonsterSkill(kvp.Key, defaultLevel, currentLevel, maximumLevel, targetForNextLevel, targetIncreaseFactor, increasePerLevel);
            }
        }

        /// <summary>
        /// Gets the type of this monster.
        /// </summary>
        public IMonsterType Type { get; }

        public uint Experience { get; }

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
        public override byte AutoAttackRange => (byte)(this.Type.Flags.HasFlag((uint)CreatureFlag.DistanceFighting) ? DistanceFightingMonsterAttackRange : MeleeFightingMonsterAttackRange);

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

        public override ChaseMode ChaseMode { get; }

        public override FightMode FightMode => FightMode.FullAttack;
    }
}
