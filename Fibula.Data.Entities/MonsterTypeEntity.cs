// -----------------------------------------------------------------
// <copyright file="MonsterTypeEntity.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Data.Entities.Contracts.Abstractions;
    using Fibula.Data.Entities.Contracts.Enumerations;
    using Fibula.Data.Entities.Contracts.Structs;

    /// <summary>
    /// Class that represents a monster type.
    /// </summary>
    public class MonsterTypeEntity : BaseEntity, IMonsterTypeEntity, ICreatureCreationMetadata
    {
        private ushort raceId;
        private string name;
        private string article;
        private uint experience;
        private ushort summonCost;
        private ushort fleeThreshold;
        private byte loseTarget;
        private Outfit outfit;
        private ushort corpse;
        private BloodType bloodType;
        private (byte switchToFirst, byte switchToLowestHp, byte switchToHigestDmgDealt, byte switchToClosest) strategy;
        private ushort baseAttack;
        private ushort baseDefense;
        private ushort baseArmorRating;
        private ushort maxHitPoints;
        private ushort baseSpeed;
        private ushort capacity;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterTypeEntity"/> class.
        /// </summary>
        public MonsterTypeEntity()
        {
            this.RaceId = 0;
            this.Name = string.Empty;
            this.MaxManapoints = 0;

            this.BaseAttack = 1;
            this.BaseDefense = 1;
            this.BaseArmorRating = 1;

            this.BaseExperienceYield = 0;
            this.SummonCost = 0;
            this.HitpointFleeThreshold = 0;
            this.LoseTargetDistance = 0;
            this.ConditionInfect = 0;

            // this.KnownSpells = new HashSet<KnownSpell>();
            this.Phrases = new List<string>();
            this.Skills = new Dictionary<SkillType, (int DefaultLevel, int CurrentLevel, int MaximumLevel, uint TargetCount, uint CountIncreaseFactor, byte IncreaserPerLevel)>();
            this.InventoryComposition = new List<(ushort, byte, ushort)>();

            this.Locked = false;
        }

        /// <summary>
        /// Gets a value indicating whether this type is locked and thus, no changes are allowed.
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// Gets or sets the id of the monster race.
        /// </summary>
        public ushort RaceId
        {
            get => this.raceId;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.RaceId)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.raceId = value;
                this.Id = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the name of the monster type.
        /// </summary>
        public string Name
        {
            get => this.name;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.Name)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.name = value.Trim('\"');
            }
        }

        /// <summary>
        /// Gets or sets the article to use with the name.
        /// </summary>
        public string Article
        {
            get => this.article;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.Article)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.article = value.Trim('\"');
            }
        }

        /// <summary>
        /// Gets or sets the amount of experience that this type of monster yields.
        /// </summary>
        public uint BaseExperienceYield
        {
            get => this.experience;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.BaseExperienceYield)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.experience = value;
            }
        }

        /// <summary>
        /// Gets or sets the base cost to summon this type of monster.
        /// </summary>
        public ushort SummonCost
        {
            get => this.summonCost;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.SummonCost)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.summonCost = value;
            }
        }

        /// <summary>
        /// Gets or sets the threshold value under which this type of monster begins to flee battle.
        /// </summary>
        public ushort HitpointFleeThreshold
        {
            get => this.fleeThreshold;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.HitpointFleeThreshold)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.fleeThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the distance in tiles after which this type of monster looses track of their target.
        /// </summary>
        public byte LoseTargetDistance
        {
            get => this.loseTarget;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.LoseTargetDistance)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.loseTarget = value;
            }
        }

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
        /// <remarks>The flags are stored as bits in a 64 bit unsigned integer.</remarks>
        public ulong Flags { get; private set; }

        /// <summary>
        /// Gets the skills that this type of monster starts with.
        /// </summary>
        public IDictionary<SkillType, (int DefaultLevel, int CurrentLevel, int MaximumLevel, uint TargetCount, uint CountIncreaseFactor, byte IncreaserPerLevel)> Skills { get; private set; }

        /// <summary>
        /// Gets the phrases that this monster type uses.
        /// </summary>
        public IList<string> Phrases { get; private set; }

        /// <summary>
        /// Gets the composition of the inventory that this type of monster has a chance to be created with.
        /// </summary>
        public IList<(ushort typeId, byte maxAmount, ushort chance)> InventoryComposition { get; private set; }

        /// <summary>
        /// Gets or sets this type of monster outfit.
        /// </summary>
        public Outfit Outfit
        {
            get => this.outfit;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.Outfit)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.outfit = value;
            }
        }

        /// <summary>
        /// Gets or sets this type of monster's corpse item type id.
        /// </summary>
        public ushort Corpse
        {
            get => this.corpse;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.Corpse)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.corpse = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum hitpoints that this monster type starts with.
        /// </summary>
        public ushort MaxHitpoints
        {
            get => this.maxHitPoints;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.MaxHitpoints)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.maxHitPoints = value;
            }
        }

        /// <summary>
        /// Gets the maximum manapoints that this monster type starts with.
        /// </summary>
        public ushort MaxManapoints { get; private set; }

        /// <summary>
        /// Gets or sets the type of blood of this monster type.
        /// </summary>
        public BloodType BloodType
        {
            get => this.bloodType;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.BloodType)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.bloodType = value;
            }
        }

        /// <summary>
        /// Gets or sets the base movement speed for this type of monster.
        /// </summary>
        public ushort BaseSpeed
        {
            get => this.baseSpeed;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.BaseSpeed)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.baseSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum capacity for this type of monster.
        /// </summary>
        public ushort Capacity
        {
            get => this.capacity;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.Capacity)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.capacity = value;
            }
        }

        /// <summary>
        /// Gets or sets the fighting strategy of this type of monster.
        /// </summary>
        public (byte switchToClosest, byte switchToLowestHp, byte switchToHigestDmgDealt, byte randomSwitch) Strategy
        {
            get => this.strategy;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.Strategy)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.strategy = value;
            }
        }

        /// <summary>
        /// Gets or sets the base attack for this type of monster.
        /// </summary>
        public ushort BaseAttack
        {
            get => this.baseAttack;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.BaseAttack)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.baseAttack = value;
            }
        }

        /// <summary>
        /// Gets or sets the base defense for this type of monster.
        /// </summary>
        public ushort BaseDefense
        {
            get => this.baseDefense;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.BaseDefense)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.baseDefense = value;
            }
        }

        /// <summary>
        /// Gets or sets the base armor rating for this type of monster.
        /// </summary>
        public ushort BaseArmorRating
        {
            get => this.baseArmorRating;

            set
            {
                if (this.Locked)
                {
                    throw new InvalidOperationException($"Unable to set {nameof(this.BaseArmorRating)}. The {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
                }

                this.baseArmorRating = value;
            }
        }

        /// <summary>
        /// Locks this type to protect from further changes.
        /// </summary>
        public void Lock()
        {
            this.Locked = true;
        }

        /// <summary>
        /// Sets a creature flag in this type's <see cref="Flags"/>.
        /// </summary>
        /// <param name="creatureFlag">The flag to set in the type.</param>
        public void SetCreatureFlag(CreatureFlag creatureFlag)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This type is locked and cannot be altered. {nameof(this.SetCreatureFlag)}({nameof(creatureFlag)}={creatureFlag}");
            }

            this.Flags |= (ulong)creatureFlag;
        }

        /// <summary>
        /// Sets a given skill for this monster type.
        /// </summary>
        /// <param name="skillType">The type of skill to set.</param>
        /// <param name="currentLevel">The current skill level.</param>
        /// <param name="defaultLevel">The default level of the skill.</param>
        /// <param name="maximumLevel">The maximum skill level.</param>
        /// <param name="targetCount">The next target count for skill advancement.</param>
        /// <param name="countIncreaseFactor">The factor by which the next target count increases upon advancement.</param>
        /// <param name="increaserPerLevel">The base increase per level upon advancement.</param>
        public void SetSkill(SkillType skillType, int currentLevel, int defaultLevel, int maximumLevel, uint targetCount, uint countIncreaseFactor, byte increaserPerLevel)
        {
            this.Skills[skillType] = (defaultLevel, currentLevel, maximumLevel, targetCount, countIncreaseFactor, increaserPerLevel);
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
                throw new InvalidOperationException($"This {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
            }

            // TODO: implement.
        }

        /// <summary>
        /// Sets the type's spells.
        /// </summary>
        /// <param name="spells">The spells of the monster type.</param>
        public void SetSpells(IEnumerable<(IEnumerable<string> conditions, IEnumerable<string> effects, string chance)> spells)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
            }

            // TODO: implement.
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
                throw new InvalidOperationException($"This {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
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
                throw new InvalidOperationException($"This {nameof(MonsterTypeEntity)} is locked and cannot be altered.");
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
