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
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents all players in the game.
    /// </summary>
    public class Monster : Creature
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
            this.Experience = monsterType.Experience;
            this.Speed += monsterType.Speed;
            this.Outfit = monsterType.Outfit;

            this.Inventory = new MonsterInventory(itemFactory, this, monsterType.InventoryComposition);

            //foreach (var kvp in this.Type.Skills.ToList())
            //{
            //    this.Type.Skills[kvp.Key] = kvp.Value;
            //}
        }

        public IMonsterType Type { get; }

        public uint Experience { get; }

        public sealed override IInventory Inventory { get; protected set; }

        public override bool CanBeMoved => !this.Type.Flags.Contains(CreatureFlag.Unpushable);

        //public override ushort AttackPower => Math.Max(this.Type.Attack, this.Inventory.TotalAttack);

        //public override ushort ArmorRating => Math.Max(this.Type.Armor, this.Inventory.TotalArmor);

        //public override ushort DefensePower => Math.Max(this.Type.Defense, this.Inventory.TotalDefense);

        //public override byte AutoAttackRange => (byte)(this.Type.Flags.Contains(CreatureFlag.DistanceFighting) ? 5 : 1);
    }
}
