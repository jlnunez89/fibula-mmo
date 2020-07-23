// -----------------------------------------------------------------
// <copyright file="MonsterSkill.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Delegates;
    using Fibula.Data.Entities.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a monster's standard skill.
    /// </summary>
    public class MonsterSkill : ISkill
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterSkill"/> class.
        /// </summary>
        /// <param name="type">This skill's type.</param>
        /// <param name="defaultLevel">This skill's default level.</param>
        /// <param name="level">This skill's current level.</param>
        /// <param name="maxLevel">This skill's maximum level.</param>
        /// <param name="targetForNextLevel">This skill's target count for next level.</param>
        /// <param name="targetIncreaseFactor">This skill's target increase factor for calculating the next level's target.</param>
        /// <param name="increasePerLevel">This skill's value increase level over level.</param>
        public MonsterSkill(SkillType type, int defaultLevel, int level, int maxLevel, uint targetForNextLevel, uint targetIncreaseFactor, byte increasePerLevel)
        {
            if (defaultLevel < 0)
            {
                throw new ArgumentException($"{nameof(defaultLevel)} must not be negative.", nameof(defaultLevel));
            }

            if (maxLevel < 1)
            {
                throw new ArgumentException($"{nameof(maxLevel)} must be positive.", nameof(maxLevel));
            }

            if (maxLevel < defaultLevel)
            {
                throw new ArgumentException($"{nameof(maxLevel)} must be at least the same value as {nameof(defaultLevel)}.", nameof(maxLevel));
            }

            this.Type = type;
            this.DefaultLevel = (uint)Math.Max(0, defaultLevel);
            this.MaxLevel = (uint)Math.Max(0, maxLevel);
            this.Level = (uint)Math.Min(this.MaxLevel, level == 0 ? defaultLevel : level);
            this.Rate = targetIncreaseFactor / 1000d;
            this.Count = 0;
            this.TargetCount = targetForNextLevel;
            this.PerLevelIncrease = increasePerLevel;
        }

        /// <summary>
        /// Event triggered when this skill advances to the next level.
        /// </summary>
        public event OnSkillAdvanced Advanced;

        /// <summary>
        /// Event triggered when this skill's percent changes.
        /// </summary>
        public event OnSkillPercentChanged PercentChanged;

        /// <summary>
        /// Gets this skill's type.
        /// </summary>
        public SkillType Type { get; }

        /// <summary>
        /// Gets this skill's level.
        /// </summary>
        public uint Level { get; private set; }

        /// <summary>
        /// Gets this skill's maximum level.
        /// </summary>
        public uint MaxLevel { get; }

        /// <summary>
        /// Gets this skill's default level.
        /// </summary>
        public uint DefaultLevel { get; }

        /// <summary>
        /// Gets this skill's current count.
        /// </summary>
        public double Count { get; private set; }

        /// <summary>
        /// Gets the value by which to advance on skill level increase.
        /// </summary>
        public byte PerLevelIncrease { get; }

        /// <summary>
        /// Gets this skill's rate of target count increase.
        /// </summary>
        public double Rate { get; }

        /// <summary>
        /// Gets this skill's target count.
        /// </summary>
        public double TargetCount { get; private set; }

        /// <summary>
        /// Gets the count at which the current level starts.
        /// </summary>
        public double StartingCountAtLevel { get; private set; }

        /// <summary>
        /// Gets this skill's target base increase level over level.
        /// </summary>
        public double BaseTargetIncrease => throw new NotImplementedException();

        /// <summary>
        /// Gets the current percentual value between current and target counts this skill.
        /// </summary>
        public byte Percent
        {
            get
            {
                var unadjustedPercent = Math.Max(0, Math.Min(this.Count / this.TargetCount, 100)) * 100;

                return (byte)Math.Floor(unadjustedPercent);
            }
        }

        /// <summary>
        /// Increases this skill's counter.
        /// </summary>
        /// <param name="value">The amount by which to increase this skills counter.</param>
        public void IncreaseCounter(double value)
        {
            var lastPercentVal = this.Percent;

            this.Count = Math.Min(this.TargetCount, this.Count + value);

            // Skill level advance
            if (Math.Abs(this.Count - this.TargetCount) < 0.001)
            {
                this.Level += this.PerLevelIncrease;

                this.StartingCountAtLevel = this.TargetCount;
                this.TargetCount = Math.Floor(this.TargetCount * this.Rate);

                // Invoke any subscribers to the level advance.
                this.Advanced?.Invoke(this.Type);
            }

            if (this.Percent != lastPercentVal)
            {
                this.PercentChanged?.Invoke(this.Type);
            }
        }
    }
}
