using System;
using System.Linq;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Monsters
{
    public class Monster : Creature
    {
        public MonsterType Type { get; }

        public uint Experience { get; }

        public sealed override IInventory Inventory { get; protected set; }

        public override bool CanBeMoved => !Type.Flags.Contains(CreatureFlag.Unpushable);

        public override ushort AttackPower => Math.Max(Type.Attack, Inventory.TotalAttack);

        public override ushort ArmorRating => Math.Max(Type.Armor, Inventory.TotalArmor);

        public override ushort DefensePower => Math.Max(Type.Defense, Inventory.TotalDefense);

        public override byte AutoAttackRange => (byte)(Type.Flags.Contains(CreatureFlag.DistanceFighting) ? 5 : 1);

        public Monster(MonsterType monsterType)
            : base(GetNewId(), monsterType.Name, monsterType.Article, monsterType.MaxHitPoints, monsterType.MaxManaPoints, monsterType.Corpse)
        {
            Type = monsterType;
            Experience = monsterType.Experience;
            Speed += monsterType.Speed;
            Outfit = monsterType.Outfit;

            Inventory = new MonsterInventory(this, monsterType.InventoryComposition);

            foreach (var kvp in Type.Skills.ToList())
            {
                Type.Skills[kvp.Key] = kvp.Value;
            }
        }
    }
}
