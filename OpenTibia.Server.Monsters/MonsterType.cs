// -----------------------------------------------------------------
// <copyright file="MonsterType.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Parsing.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a monster type.
    /// </summary>
    public class MonsterType : IMonsterType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterType"/> class.
        /// </summary>
        public MonsterType()
        {
            this.RaceId = 0;
            this.Name = string.Empty;
            this.MaxManaPoints = 0;

            this.Attack = 1;
            this.Defense = 1;
            this.Armor = 1;

            this.Experience = 0;
            this.SummonCost = 0;
            this.FleeThreshold = 0;
            this.LoseTarget = 0;
            this.ConditionInfect = 0;

            // this.KnownSpells = new HashSet<KnownSpell>();
            this.Flags = (uint)CreatureFlag.None;
            this.Phrases = new List<string>();
            this.Skills = new Dictionary<SkillType, (int CurrentLevel, int DefaultLevel, int MaximumLevel, uint CurrentCount, uint CountForNextLevel, byte AddOnLevel)>();
            this.InventoryComposition = new List<(ushort, byte, ushort)>();

            this.Locked = false;
        }

        /// <summary>
        /// Gets a value indicating whether this type is locked and thus, no changes are allowed.
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// Gets the id of the monster race.
        /// </summary>
        public ushort RaceId { get; private set; }

        /// <summary>
        /// Gets the name of the monster type.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the article to use with the name.
        /// </summary>
        public string Article { get; private set; }

        /// <summary>
        /// Gets the amount of experience that this type of monster deals.
        /// </summary>
        public uint Experience { get; private set; }

        /// <summary>
        /// Gets the base cost to summon this type of monster.
        /// </summary>
        public ushort SummonCost { get; private set; }

        /// <summary>
        /// Gets the threshold value under which this type of monster begins to flee battle.
        /// </summary>
        public ushort FleeThreshold { get; private set; }

        /// <summary>
        /// Gets the distance in tiles after which this type of monster looses track of their target.
        /// </summary>
        public byte LoseTarget { get; private set; }

        /// <summary>
        /// Gets an encoded value containing <see cref="ConditionFlag"/>s, detailing if this type of monster infects upon
        /// successfuly dealing basic damange in combat.
        /// </summary>
        public ushort ConditionInfect { get; private set; }

        ///// <summary>
        ///// Gets a collection of spells known by this type of monster.
        ///// </summary>
        // public ISet<ISpell> KnownSpells { get; private set; }

        /// <summary>
        /// Gets the flags set for this type of monster.
        /// </summary>
        public uint Flags { get; private set; }

        /// <summary>
        /// Gets the skills that this type of monster starts with.
        /// </summary>
        public IDictionary<SkillType, (int CurrentLevel, int DefaultLevel, int MaximumLevel, uint CurrentCount, uint CountForNextLevel, byte AddOnLevel)> Skills { get; private set; }

        /// <summary>
        /// Gets the phrases that this monster type uses.
        /// </summary>
        public IList<string> Phrases { get; private set; }

        /// <summary>
        /// Gets the composition of the inventory that this type of monster has a chance to be created with.
        /// </summary>
        public IList<(ushort typeId, byte maxAmount, ushort chance)> InventoryComposition { get; private set; }

        /// <summary>
        /// Gets this type of monster outfit.
        /// </summary>
        public Outfit Outfit { get; private set; }

        /// <summary>
        /// Gets this type of monster's corpse item type id.
        /// </summary>
        public ushort Corpse { get; private set; }

        /// <summary>
        /// Gets the maximum hitpoints that this monster type starts with.
        /// </summary>
        public ushort MaxHitPoints { get; private set; }

        /// <summary>
        /// Gets the maximum manapoints that this monster type starts with.
        /// </summary>
        public ushort MaxManaPoints { get; private set; }

        /// <summary>
        /// Gets the type of blood of this monster type.
        /// </summary>
        public BloodType Blood { get; private set; }

        /// <summary>
        /// Gets the base movement speed for this type of monster.
        /// </summary>
        public ushort Speed { get; private set; }

        /// <summary>
        /// Gets the maximum capacity for this type of monster.
        /// </summary>
        public ushort Capacity { get; private set; }

        /// <summary>
        /// Gets the fighting strategy of this type of monster.
        /// </summary>
        public (byte switchToFirstChance, byte switchToLowestHpChance, byte switchToHigestDmgDealtChance, byte switchToClosestChance) Strategy { get; private set; }

        public ushort Attack { get; private set; }

        public ushort Defense { get; private set; }

        public ushort Armor { get; private set; }

        /// <summary>
        /// Locks this type to protect from further changes.
        /// </summary>
        public void Lock()
        {
            this.Locked = true;
        }

        /// <summary>
        /// Sets the type's id.
        /// </summary>
        /// <param name="typeId">The id of the monster type.</param>
        public void SetId(ushort typeId)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            this.RaceId = typeId;
        }

        /// <summary>
        /// Sets the type's name.
        /// </summary>
        /// <param name="name">The name of the monster type.</param>
        public void SetName(string name)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            this.Name = name;
        }

        /// <summary>
        /// Sets the type's article in the name.
        /// </summary>
        /// <param name="article">The article in the name of the monster type.</param>
        public void SetArticle(string article)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            this.Article = article;
        }

        /// <summary>
        /// Sets the type's outfit.
        /// </summary>
        /// <param name="outfitStr">The string representation of the outfit of the monster type.</param>
        public void SetOutfit(string outfitStr)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            // comes in the form
            // 68, 0-0-0-0
            var splitStr = outfitStr.Split(new[] { ',' }, 2);
            var outfitId = Convert.ToUInt16(splitStr[0]);

            var outfitSections = splitStr[1].Split('-').Select(s => Convert.ToByte(s)).ToArray();

            if (outfitId == 0)
            {
                this.Outfit = new Outfit
                {
                    Id = outfitId,
                    ItemIdLookAlike = outfitSections[0],
                };
            }
            else
            {
                this.Outfit = new Outfit
                {
                    Id = outfitId,
                    Head = outfitSections[0],
                    Body = outfitSections[1],
                    Legs = outfitSections[2],
                    Feet = outfitSections[3],
                };
            }
        }

        /// <summary>
        /// Sets the type's corpse.
        /// </summary>
        /// <param name="corpse">The corpse of the monster type.</param>
        public void SetCorpse(ushort corpse)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            this.Corpse = corpse;
        }

        /// <summary>
        /// Sets the type's type of blood.
        /// </summary>
        /// <param name="typeOfBlood">The type of blood of the monster type.</param>
        public void SetBlood(string typeOfBlood)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            if (Enum.TryParse(typeOfBlood, out BloodType bloodType))
            {
                this.Blood = bloodType;
            }
        }

        /// <summary>
        /// Sets the type's experience.
        /// </summary>
        /// <param name="experience">The experience given by this monster type.</param>
        public void SetExperience(uint experience)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            this.Experience = experience;
        }

        /// <summary>
        /// Sets the type's summon cost.
        /// </summary>
        /// <param name="summonCost">The cost to summon the monster type.</param>
        public void SetSummonCost(ushort summonCost)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            this.SummonCost = summonCost;
        }

        /// <summary>
        /// Sets the type's flee threshold.
        /// </summary>
        /// <param name="fleeThreshold">The flee threshold of the monster type.</param>
        public void SetFleeTreshold(ushort fleeThreshold)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            this.FleeThreshold = fleeThreshold;
        }

        /// <summary>
        /// Sets the type's base defense power.
        /// </summary>
        /// <param name="defense">The base defense value of the monster type.</param>
        public void SetDefense(ushort defense)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            this.Defense = defense;
        }

        /// <summary>
        /// Sets the type's base armor.
        /// </summary>
        /// <param name="armor">The base armor of the monster type.</param>
        public void SetArmor(ushort armor)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            this.Armor = armor;
        }

        /// <summary>
        /// Sets the type's base attack power.
        /// </summary>
        /// <param name="attack">The base attack power of the monster type.</param>
        public void SetAttack(ushort attack)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            this.Attack = attack;
        }

        /// <summary>
        /// Sets a type's infection condition flag.
        /// </summary>
        /// <param name="condition">The type of condition flag.</param>
        /// <param name="value">The base value by which to infect.</param>
        public void SetConditionInfect(ConditionFlag condition, ushort value)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            // TODO: implement.
        }

        /// <summary>
        /// Sets the type's lost target threshold.
        /// </summary>
        /// <param name="loseTarget">The threshold at which the monster type loses their target.</param>
        public void SetLoseTarget(byte loseTarget)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            this.LoseTarget = loseTarget;
        }

        /// <summary>
        /// Sets the type's strategy.
        /// </summary>
        /// <param name="strategy">The strategy of the monster type.</param>
        public void SetStrategy((byte fChance, byte lChance, byte hChance, byte cChance) strategy)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            this.Strategy = strategy;
        }

        /// <summary>
        /// Sets the type's flags.
        /// </summary>
        /// <param name="flagParsed">The parsed flags of the monster type.</param>
        public void SetFlags(IEnumerable<IParsedElement> flagParsed)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            foreach (var element in flagParsed)
            {
                if (!element.IsFlag || element.Attributes == null || !element.Attributes.Any())
                {
                    continue;
                }

                if (Enum.TryParse(element.Attributes.First().Name, out CreatureFlag creatureFlag))
                {
                    this.Flags.SetFlag((uint)creatureFlag);
                }
            }
        }

        /// <summary>
        /// Sets the type's spells.
        /// </summary>
        /// <param name="spells">The spells of the monster type.</param>
        public void SetSpells(IEnumerable<(IEnumerable<string> conditions, IEnumerable<string> effects, string chance)> spells)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            // TODO: implement.
        }

        /// <summary>
        /// Sets the type's skills.
        /// </summary>
        /// <param name="skillParsed">The skills of the monster type.</param>
        public void SetSkills(IEnumerable<(string Name, int CurrentLevel, int DefaultLevel, int MaximumLevel, uint CurrentCount, uint CountForNextLevel, byte AddOnLevel)> skillParsed)
        {
            skillParsed.ThrowIfNull(nameof(skillParsed));

            if (!skillParsed.Any())
            {
                throw new ArgumentException("Empty skills parsed!", nameof(skillParsed));
            }

            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            foreach (var skill in skillParsed)
            {
                if (!Enum.TryParse(skill.Name, ignoreCase: true, out MonsterSkillType mSkill))
                {
                    continue;
                }

                switch (mSkill)
                {
                    case MonsterSkillType.Hitpoints:
                        this.MaxHitPoints = skill.CurrentLevel < 0 ? ushort.MaxValue : (ushort)skill.CurrentLevel;
                        break;
                    case MonsterSkillType.GoStrength:
                        this.Speed = skill.CurrentLevel < 0 ? ushort.MinValue : (ushort)skill.CurrentLevel;
                        break;
                    case MonsterSkillType.CarryStrength:
                        this.Capacity = skill.CurrentLevel < 0 ? ushort.MinValue : (ushort)skill.CurrentLevel;
                        break;
                    case MonsterSkillType.FistFighting:
                        if (skill.CurrentLevel > 0)
                        {
                            this.Skills[SkillType.NoWeapon] = (skill.CurrentLevel, skill.DefaultLevel, skill.MaximumLevel, skill.CurrentCount, skill.CountForNextLevel, skill.AddOnLevel);
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Sets the type's inventory.
        /// </summary>
        /// <param name="possibleInventory">The inventory of the monster type.</param>
        public void SetInventory(IEnumerable<(ushort typeId, byte maxAmount, ushort dropChance)> possibleInventory)
        {
            possibleInventory.ThrowIfNull(nameof(possibleInventory));

            if (!possibleInventory.Any())
            {
                // no loot on this one.
                return;
            }

            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            foreach (var entry in possibleInventory)
            {
                this.InventoryComposition.Add(entry);
            }
        }

        /// <summary>
        /// Sets the type's phrases.
        /// </summary>
        /// <param name="phrases">The phrases of the monster type.</param>
        public void SetPhrases(IEnumerable<string> phrases)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterType)} is locked and cannot be altered.");
            }

            if (phrases == null || !phrases.Any())
            {
                return;
            }

            foreach (var phrase in phrases)
            {
                if (string.IsNullOrWhiteSpace(phrase))
                {
                    continue;
                }

                this.Phrases.Add(phrase);
            }
        }
    }
}