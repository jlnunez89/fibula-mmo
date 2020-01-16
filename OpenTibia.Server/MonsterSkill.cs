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
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
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
        /// <param name="rate">This skill's rate of target count increase.</param>
        /// <param name="level">This skill's current level.</param>
        /// <param name="maxLevel">This skill's maximum level.</param>
        /// <param name="count">This skill's current count.</param>
        /// <param name="baseIncrease">This skill's target base increase level over level.</param>
        /// <param name="skillIncrease">This skill's value increase level over level.</param>
        public MonsterSkill(SkillType type, ushort defaultLevel, double rate, ushort level, ushort maxLevel, uint count, uint baseIncrease, byte skillIncrease)
        {
            if (defaultLevel < 1)
            {
                throw new ArgumentException($"{nameof(defaultLevel)} must be positive.", nameof(defaultLevel));
            }

            if (maxLevel < 1)
            {
                throw new ArgumentException($"{nameof(maxLevel)} must be positive.", nameof(maxLevel));
            }

            if (rate < 1)
            {
                throw new ArgumentException($"{nameof(rate)} must be positive.", nameof(rate));
            }

            if (baseIncrease < 1)
            {
                throw new ArgumentException($"{nameof(baseIncrease)} must be positive.", nameof(baseIncrease));
            }

            if (count < 0)
            {
                throw new ArgumentException($"{nameof(count)} cannot be negative.", nameof(count));
            }

            if (maxLevel < defaultLevel)
            {
                throw new ArgumentException($"{nameof(maxLevel)} must be at least the same value as {nameof(defaultLevel)}.", nameof(maxLevel));
            }

            this.Type = type;
            this.DefaultLevel = defaultLevel;
            this.MaxLevel = maxLevel;
            this.Level = Math.Min(this.MaxLevel, level == 0 ? defaultLevel : level);
            this.Rate = rate;
            this.BaseTargetIncrease = baseIncrease;

            this.Target = this.CalculateNextTarget();
            this.Count = Math.Min(count, this.Target);
            this.PerLevelIncrease = skillIncrease;
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
        public ushort Level { get; private set; }

        /// <summary>
        /// Gets this skill's maximum level.
        /// </summary>
        public ushort MaxLevel { get; }

        /// <summary>
        /// Gets this skill's default level.
        /// </summary>
        public ushort DefaultLevel { get; }

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
        public double BaseTargetIncrease { get; }

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
                this.Target = this.CalculateNextTarget();

                // Invoke any subscribers to the level advance.
                this.OnAdvance?.Invoke(this.Type);
            }
        }

        /// <summary>
        /// Calculates the next target count.
        /// </summary>
        /// <returns>The next target count value.</returns>
        private double CalculateNextTarget()
        {
            var nextTarget = (this.Target * this.Rate) + this.BaseTargetIncrease;

            // need to recalculate everything.
            if (Math.Abs(this.Target) < 0.001)
            {
                for (int i = 0; i < this.DefaultLevel - this.Level; i++)
                {
                    // how many advances we need to calculate
                    nextTarget = (nextTarget * this.Rate) + this.BaseTargetIncrease;
                }
            }

            return nextTarget;
        }
    }
}