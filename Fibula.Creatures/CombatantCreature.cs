// -----------------------------------------------------------------
// <copyright file="CombatantCreature.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Combat.Enumerations;
    using Fibula.Mechanics.Contracts.Constants;
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Class that represents all creatures in the game.
    /// </summary>
    public abstract class CombatantCreature : Creature, ICombatant
    {
        /// <summary>
        /// An object to use as a lock to semaphone changes to the <see cref="damageTakenFromOthers"/> dictionary.
        /// </summary>
        private readonly object damageTakenFromOthersLock;

        /// <summary>
        /// Lock object to semaphore interaction with the exhaustion dictionary.
        /// </summary>
        private readonly object exhaustionLock;

        /// <summary>
        /// Stores the map of other combatants to the damage taken from them.
        /// </summary>
        private readonly Dictionary<uint, uint> damageTakenFromOthers;

        /// <summary>
        /// The base of how fast a combatant can earn an attack credit per combat round.
        /// </summary>
        private readonly decimal baseAttackSpeed;

        /// <summary>
        /// The base of how fast a combatant can earn a defense credit per combat round.
        /// </summary>
        private readonly decimal baseDefenseSpeed;

        /// <summary>
        /// The buff for attack speed.
        /// </summary>
        private decimal attackSpeedBuff;

        /// <summary>
        /// The buff for defense speed.
        /// </summary>
        private decimal defenseSpeedBuff;

        /// <summary>
        /// Initializes a new instance of the <see cref="CombatantCreature"/> class.
        /// </summary>
        /// <param name="name">The name of this creature.</param>
        /// <param name="article">An article for the name of this creature.</param>
        /// <param name="maxHitpoints">The maximum hitpoints of the creature.</param>
        /// <param name="maxManapoints">The maximum manapoints of the creature.</param>
        /// <param name="corpse">The corpse of the creature.</param>
        /// <param name="hitpoints">The current hitpoints of the creature.</param>
        /// <param name="manapoints">The current manapoints of the creature.</param>
        /// <param name="baseAttackSpeed">
        /// Optional. The base attack speed for this creature.
        /// Bounded between [<see cref="CombatConstants.MinimumCombatSpeed"/>, <see cref="CombatConstants.MaximumCombatSpeed"/>] inclusive.
        /// Defaults to <see cref="CombatConstants.DefaultAttackSpeed"/>.
        /// </param>
        /// <param name="baseDefenseSpeed">
        /// Optional. The base defense speed for this creature.
        /// Bounded between [<see cref="CombatConstants.MinimumCombatSpeed"/>, <see cref="CombatConstants.MaximumCombatSpeed"/>] inclusive.
        /// Defaults to <see cref="CombatConstants.DefaultDefenseSpeed"/>.
        /// </param>
        protected CombatantCreature(
            string name,
            string article,
            ushort maxHitpoints,
            ushort maxManapoints,
            ushort corpse,
            ushort hitpoints = 0,
            ushort manapoints = 0,
            decimal baseAttackSpeed = CombatConstants.DefaultAttackSpeed,
            decimal baseDefenseSpeed = CombatConstants.DefaultDefenseSpeed)
            : base(name, article, maxHitpoints, maxManapoints, corpse, hitpoints, manapoints)
        {
            // Normalize combat speeds.
            this.baseAttackSpeed = Math.Min(CombatConstants.MaximumCombatSpeed, Math.Max(CombatConstants.MinimumCombatSpeed, baseAttackSpeed));
            this.baseDefenseSpeed = Math.Min(CombatConstants.MaximumCombatSpeed, Math.Max(CombatConstants.MinimumCombatSpeed, baseDefenseSpeed));

            this.AutoAttackCredits = this.AutoAttackMaximumCredits;
            this.AutoDefenseCredits = this.AutoDefenseMaximumCredits;

            this.exhaustionLock = new object();
            this.ExhaustionInformation = new Dictionary<ExhaustionType, DateTimeOffset>();

            this.damageTakenFromOthersLock = new object();
            this.damageTakenFromOthers = new Dictionary<uint, uint>();
        }

        /// <summary>
        /// Gets the current target combatant.
        /// </summary>
        public ICombatant AutoAttackTarget { get; private set; }

        /// <summary>
        /// Gets the number of attack credits available.
        /// </summary>
        public int AutoAttackCredits { get; private set; }

        /// <summary>
        /// Gets the number of maximum attack credits.
        /// </summary>
        public ushort AutoAttackMaximumCredits => CombatConstants.DefaultMaximumAttackCredits;

        /// <summary>
        /// Gets the number of auto defense credits available.
        /// </summary>
        public int AutoDefenseCredits { get; private set; }

        /// <summary>
        /// Gets the number of maximum defense credits.
        /// </summary>
        public ushort AutoDefenseMaximumCredits => CombatConstants.DefaultMaximumDefenseCredits;

        /// <summary>
        /// Gets a metric of how fast a combatant can earn an attack credit per combat round.
        /// </summary>
        public decimal AttackSpeed => this.baseAttackSpeed + this.attackSpeedBuff;

        /// <summary>
        /// Gets a metric of how fast a combatant can earn a defense credit per combat round.
        /// </summary>
        public decimal DefenseSpeed => this.baseDefenseSpeed + this.defenseSpeedBuff;

        /// <summary>
        /// Gets the target being chased, if any.
        /// </summary>
        public ICombatant ChasingTarget { get; private set; }

        /// <summary>
        /// Gets or sets the chase mode selected by this combatant.
        /// </summary>
        public ChaseMode ChaseMode { get; set; }

        /// <summary>
        /// Gets or sets the fight mode selected by this combatant.
        /// </summary>
        public FightMode FightMode { get; set; }

        /// <summary>
        /// Gets the range that the auto attack has.
        /// </summary>
        public abstract byte AutoAttackRange { get; }

        /// <summary>
        /// Gets the distribution of damage taken by any combatant that has attacked this combatant while the current combat is active.
        /// </summary>
        public IEnumerable<(uint, uint)> DamageTakenDistribution
        {
            get
            {
                lock (this.damageTakenFromOthersLock)
                {
                    return this.damageTakenFromOthers.Select(kvp => (kvp.Key, kvp.Value)).ToList();
                }
            }
        }

        /// <summary>
        /// Gets the collection of ids of attackers of this combatant.
        /// </summary>
        public IEnumerable<uint> AttackedBy
        {
            get
            {
                lock (this.damageTakenFromOthersLock)
                {
                    return this.damageTakenFromOthers.Keys.ToList();
                }
            }
        }

        /// <summary>
        /// Gets the current exhaustion information for the entity.
        /// </summary>
        /// <remarks>
        /// The key is a <see cref="ExhaustionType"/>, and the value is a <see cref="DateTimeOffset"/>: the date and time
        /// at which exhaustion is completely recovered.
        /// </remarks>
        public IDictionary<ExhaustionType, DateTimeOffset> ExhaustionInformation { get; }

        /// <summary>
        /// Gets or sets the pending auto attack operation of this player, if any.
        /// </summary>
        public IOperation PendingAutoAttackOperation { get; set; }

        /// <summary>
        /// Consumes combat credits to the combatant.
        /// </summary>
        /// <param name="creditType">The type of combat credits to consume.</param>
        /// <param name="amount">The amount of credits to consume.</param>
        public void ConsumeCredits(CombatCreditType creditType, byte amount)
        {
            switch (creditType)
            {
                case CombatCreditType.Attack:
                    this.AutoAttackCredits -= amount;
                    break;

                case CombatCreditType.Defense:
                    this.AutoDefenseCredits -= amount;
                    break;
            }
        }

        /// <summary>
        /// Restores combat credits to the combatant.
        /// </summary>
        /// <param name="creditType">The type of combat credits to restore.</param>
        /// <param name="amount">The amount of credits to restore.</param>
        public void RestoreCredits(CombatCreditType creditType, byte amount)
        {
            switch (creditType)
            {
                case CombatCreditType.Attack:
                    this.AutoAttackCredits = Math.Min(this.AutoAttackMaximumCredits, this.AutoAttackCredits + amount);
                    break;

                case CombatCreditType.Defense:
                    this.AutoDefenseCredits = Math.Min(this.AutoDefenseMaximumCredits, this.AutoDefenseCredits + amount);
                    break;
            }
        }

        /// <summary>
        /// Sets the attack target of this combatant.
        /// </summary>
        /// <param name="otherCombatant">The other target combatant, if any.</param>
        /// <returns>True if the target was actually changed, false otherwise.</returns>
        public bool SetAttackTarget(ICombatant otherCombatant)
        {
            bool targetWasChanged = false;

            if (otherCombatant != this.AutoAttackTarget)
            {
                var oldTarget = this.AutoAttackTarget;

                this.AutoAttackTarget = otherCombatant;

                if (this.ChaseMode == ChaseMode.Chase || otherCombatant == null)
                {
                    this.ChasingTarget = otherCombatant;
                }

                // this.TargetChanged?.Invoke(this, oldTarget);
                targetWasChanged = true;
            }

            return targetWasChanged;
        }

        /// <summary>
        /// Calculates the remaining <see cref="TimeSpan"/> until the entity's exhaustion is recovered from.
        /// </summary>
        /// <param name="type">The type of exhaustion.</param>
        /// <param name="currentTime">The current time to calculate from.</param>
        /// <returns>The <see cref="TimeSpan"/> result.</returns>
        public TimeSpan CalculateRemainingCooldownTime(ExhaustionType type, DateTimeOffset currentTime)
        {
            lock (this.exhaustionLock)
            {
                if (!this.ExhaustionInformation.TryGetValue(type, out DateTimeOffset readyAtTime))
                {
                    return TimeSpan.Zero;
                }

                var timeLeft = readyAtTime - currentTime;

                if (timeLeft < TimeSpan.Zero)
                {
                    this.ExhaustionInformation.Remove(type);

                    return TimeSpan.Zero;
                }

                return timeLeft;
            }
        }

        /// <summary>
        /// Adds exhaustion of the given type.
        /// </summary>
        /// <param name="type">The type of exhaustion to add.</param>
        /// <param name="fromTime">The reference time from which to add.</param>
        /// <param name="timeSpan">The amount of time to add exhaustion for.</param>
        public void AddExhaustion(ExhaustionType type, DateTimeOffset fromTime, TimeSpan timeSpan)
        {
            lock (this.exhaustionLock)
            {
                if (this.ExhaustionInformation.ContainsKey(type) && this.ExhaustionInformation[type] > fromTime)
                {
                    fromTime = this.ExhaustionInformation[type];
                }

                this.ExhaustionInformation[type] = fromTime + timeSpan;
            }
        }

        /// <summary>
        /// Adds exhaustion of the given type.
        /// </summary>
        /// <param name="type">The type of exhaustion to add.</param>
        /// <param name="fromTime">The reference time from which to add.</param>
        /// <param name="milliseconds">The amount of time in milliseconds to add exhaustion for.</param>
        public void AddExhaustion(ExhaustionType type, DateTimeOffset fromTime, uint milliseconds)
        {
            this.AddExhaustion(type, fromTime, TimeSpan.FromMilliseconds(milliseconds));
        }

        /// <summary>
        /// Increases the attack speed of this combatant.
        /// </summary>
        /// <param name="increaseAmount">The amount by which to increase.</param>
        public void IncreaseAttackSpeed(decimal increaseAmount)
        {
            this.attackSpeedBuff = Math.Min(CombatConstants.MaximumCombatSpeed - this.baseAttackSpeed, this.attackSpeedBuff + increaseAmount);
        }

        /// <summary>
        /// Decreases the attack speed of this combatant.
        /// </summary>
        /// <param name="decreaseAmount">The amount by which to decrease.</param>
        public void DecreaseAttackSpeed(decimal decreaseAmount)
        {
            this.attackSpeedBuff = Math.Max(0, this.attackSpeedBuff - decreaseAmount);
        }

        /// <summary>
        /// Increases the defense speed of this combatant.
        /// </summary>
        /// <param name="increaseAmount">The amount by which to increase.</param>
        public void IncreaseDefenseSpeed(decimal increaseAmount)
        {
            this.defenseSpeedBuff = Math.Min(CombatConstants.MaximumCombatSpeed - this.baseDefenseSpeed, this.defenseSpeedBuff + increaseAmount);
        }

        /// <summary>
        /// Decreases the defense speed of this combatant.
        /// </summary>
        /// <param name="decreaseAmount">The amount by which to decrease.</param>
        public void DecreaseDefenseSpeed(decimal decreaseAmount)
        {
            this.defenseSpeedBuff = Math.Max(0, this.defenseSpeedBuff - decreaseAmount);
        }
    }
}
