// -----------------------------------------------------------------
// <copyright file="MonsterSkill.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Delegates;
    using OpenTibia.Server.Contracts.Enumerations;

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
            if (defaultLevel < 1)
            {
                throw new ArgumentException($"{nameof(defaultLevel)} must be positive.", nameof(defaultLevel));
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
            this.Target = targetForNextLevel;
            this.PerLevelIncrease = increasePerLevel;
        }

        /// <summary>
        /// Event triggered when this skill advances to the next level.
        /// </summary>
        public event SkillLevelAdvance OnAdvance;

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
        public double Target { get; private set; }

        /// <summary>
        /// Gets this skill's target base increase level over level.
        /// </summary>
        public double BaseTargetIncrease => throw new NotImplementedException();

        /// <summary>
        /// Increases this skill's counter.
        /// </summary>
        /// <param name="value">The amount by which to increase this skills counter.</param>
        public void IncreaseCounter(double value)
        {
            this.Count = Math.Min(this.Target, this.Count + value);

            // Skill level advance
            if (Math.Abs(this.Count - this.Target) < 0.001)
            {
                this.Level += this.PerLevelIncrease;
                this.Target = Math.Floor(this.Target * this.Rate);

                // Invoke any subscribers to the level advance.
                this.OnAdvance?.Invoke(this.Type);
            }
        }
    }
}