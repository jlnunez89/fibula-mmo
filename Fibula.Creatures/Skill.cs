// -----------------------------------------------------------------
// <copyright file="Skill.cs" company="2Dudes">
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
    /// Class that represents a creature's standard skill.
    /// </summary>
    public class Skill : ISkill
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Skill"/> class.
        /// </summary>
        /// <param name="type">This skill's type.</param>
        /// <param name="defaultLevel">This skill's default level.</param>
        /// <param name="rate">This skill's rate of target count increase.</param>
        /// <param name="baseIncrease">This skill's target base increase level over level.</param>
        /// <param name="level">This skill's current level.</param>
        /// <param name="maxLevel">This skill's maximum level.</param>
        /// <param name="count">This skill's current count.</param>
        public Skill(SkillType type, uint defaultLevel, double rate, double baseIncrease, uint level = 0, uint maxLevel = 1, double count = 0)
        {
            if (defaultLevel < 0)
            {
                throw new ArgumentException($"{nameof(defaultLevel)} must not be negative.", nameof(defaultLevel));
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

            var (startLevelCount, targetCount) = this.CalculateNextTarget();

            this.StartingCountAtLevel = startLevelCount;
            this.TargetCount = targetCount;

            this.Count = Math.Min(count, this.TargetCount);
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
        /// Gets this skill's rate of target count increase.
        /// </summary>
        public double Rate { get; }

        /// <summary>
        /// Gets the count at which the current level starts.
        /// </summary>
        public double StartingCountAtLevel { get; private set; }

        /// <summary>
        /// Gets this skill's target count.
        /// </summary>
        public double TargetCount { get; private set; }

        /// <summary>
        /// Gets this skill's target base increase level over level.
        /// </summary>
        public double BaseTargetIncrease { get; }

        /// <summary>
        /// Gets the current percentual value between current and target counts this skill.
        /// </summary>
        public byte Percent
        {
            get
            {
                var fromCount = Math.Max(0, this.Count - this.StartingCountAtLevel);
                var toCount = Math.Max(1, this.TargetCount - this.StartingCountAtLevel);

                var unadjustedPercent = Math.Max(0, Math.Min(fromCount / toCount, 100)) * 100;

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
                this.Level++;

                var (startLevelCount, targetCount) = this.CalculateNextTarget();

                this.StartingCountAtLevel = startLevelCount;
                this.TargetCount = targetCount;

                // Invoke any subscribers to the level advance.
                this.Advanced?.Invoke(this.Type);
            }

            if (this.Percent != lastPercentVal)
            {
                this.PercentChanged?.Invoke(this.Type);
            }
        }

        /// <summary>
        /// Calculates the next target count.
        /// </summary>
        /// <returns>A tuple cointaining the current level's starting count, and the next target count.</returns>
        private (double currentLevelStartCount, double targetCountForNextLevel) CalculateNextTarget()
        {
            var currentLevelStartCount = this.TargetCount;
            var nextTarget = (this.TargetCount * this.Rate) + this.BaseTargetIncrease;

            // need to recalculate everything.
            if (Math.Abs(this.TargetCount) < 0.001)
            {
                for (int i = 0; i < this.Level - this.DefaultLevel; i++)
                {
                    currentLevelStartCount = nextTarget;
                    nextTarget = (nextTarget * this.Rate) + this.BaseTargetIncrease;
                }
            }

            return (currentLevelStartCount, nextTarget);
        }
    }
}
