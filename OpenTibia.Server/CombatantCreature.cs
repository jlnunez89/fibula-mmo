// -----------------------------------------------------------------
// <copyright file="CombatantCreature.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Delegates;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents all creatures in the game.
    /// </summary>
    public abstract class CombatantCreature : Creature, ICombatant
    {
        private const decimal DefaultAttackSpeed = 1.0m;

        private const decimal DefaultDefenseSpeed = 2.0m;

        private const byte DefaultMaximumAttackCredits = 1;

        private const byte DefaultMaximumDefenseCredits = 2;

        private readonly object damageTakenFromOthersLock;

        private readonly Dictionary<uint, uint> damageTakenFromOthers;

        /// <summary>
        /// Initializes a new instance of the <see cref="CombatantCreature"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="article"></param>
        /// <param name="maxHitpoints"></param>
        /// <param name="maxManapoints"></param>
        /// <param name="corpse"></param>
        /// <param name="hitpoints"></param>
        /// <param name="manapoints"></param>
        /// <param name="baseAttackSpeed"></param>
        /// <param name="baseDefenseSpeed"></param>
        protected CombatantCreature(
            string name,
            string article,
            ushort maxHitpoints,
            ushort maxManapoints,
            ushort corpse,
            ushort hitpoints = 0,
            ushort manapoints = 0,
            decimal baseAttackSpeed = DefaultAttackSpeed,
            decimal baseDefenseSpeed = DefaultDefenseSpeed)
            : base(name, article, maxHitpoints, maxManapoints, corpse, hitpoints, manapoints)
        {
            // Normalize combat speeds.
            this.BaseAttackSpeed = Math.Min(ICombatant.MaximumCombatSpeed, Math.Max(ICombatant.MinimumCombatSpeed, baseAttackSpeed));
            this.BaseDefenseSpeed = Math.Min(ICombatant.MaximumCombatSpeed, Math.Max(ICombatant.MinimumCombatSpeed, baseDefenseSpeed));

            this.AutoAttackCredits = this.AutoAttackMaximumCredits;
            this.AutoDefenseCredits = this.AutoDefenseMaximumCredits;

            this.damageTakenFromOthersLock = new object();
            this.damageTakenFromOthers = new Dictionary<uint, uint>();

            this.HostilesInView = new HashSet<uint>();
            this.NeutralsInView = new HashSet<uint>();
            this.FriendlyInView = new HashSet<uint>();
        }

        /// <summary>
        /// Event to call when the attack target changes.
        /// </summary>
        public event OnAttackTargetChange TargetChanged;

        /// <summary>
        /// Event to call when the fight mode changes.
        /// </summary>
        public event FightModeChanged FightModeChanged;

        /// <summary>
        /// Event to call when the chase mode changes.
        /// </summary>
        public event ChaseModeChanged ChaseModeChanged;

        /// <summary>
        /// Event to call when a combat credit is consumed.
        /// </summary>
        public event CombatCreditConsumed CombatCreditsConsumed;

        /// <summary>
        /// Event to call when combat starts for this combatant.
        /// </summary>
        public event CombatStarted CombatStarted;

        /// <summary>
        /// Event to call when combat ends for this combatant.
        /// </summary>
        public event CombatEnded CombatEnded;

        /// <summary>
        /// Gets the set of creatures currently in view for this combatant.
        /// </summary>
        public IEnumerable<uint> CreaturesInView
        {
            get
            {
                return this.HostilesInView
                    .Union(this.NeutralsInView)
                    .Union(this.FriendlyInView);
            }
        }

        /// <summary>
        /// Gets the set of ids of creatures that this combatant considers hostile, and tipically initiates combat against.
        /// </summary>
        public ISet<uint> HostilesInView { get; }

        /// <summary>
        /// Gets the set of ids of creatures that this combatant considers neutral.
        /// </summary>
        public ISet<uint> NeutralsInView { get; }

        /// <summary>
        /// Gets the set of ids of creatures that this combatant considers friendly, and tipically treats favorably.
        /// </summary>
        public ISet<uint> FriendlyInView { get; }

        /// <summary>
        /// Gets the current target combatant.
        /// </summary>
        public ICombatant AutoAttackTarget { get; private set; }

        /// <summary>
        /// Gets the number of attack credits available.
        /// </summary>
        public byte AutoAttackCredits { get; private set; }

        /// <summary>
        /// Gets the number of maximum attack credits.
        /// </summary>
        public byte AutoAttackMaximumCredits => DefaultMaximumAttackCredits;

        /// <summary>
        /// Gets the number of auto defense credits available.
        /// </summary>
        public byte AutoDefenseCredits { get; private set; }

        /// <summary>
        /// Gets the number of maximum defense credits.
        /// </summary>
        public byte AutoDefenseMaximumCredits => DefaultMaximumDefenseCredits;

        /// <summary>
        /// Gets a metric of how fast an Actor can earn a new AutoAttack credit per second.
        /// </summary>
        public decimal BaseAttackSpeed { get; }

        /// <summary>
        /// Gets a metric of how fast an Actor can earn a new AutoDefense credit per second.
        /// </summary>
        public decimal BaseDefenseSpeed { get; }

        /// <summary>
        /// Gets the target being chased, if any.
        /// </summary>
        public ICombatant ChasingTarget { get; private set; }

        /// <summary>
        /// Gets or sets the chase mode selected by this combatant.
        /// </summary>
        public abstract ChaseMode ChaseMode { get; set; }

        /// <summary>
        /// Gets or sets the fight mode selected by this combatant.
        /// </summary>
        public abstract FightMode FightMode { get; set; }

        /// <summary>
        /// Gets the range that the auto attack has.
        /// </summary>
        public abstract byte AutoAttackRange { get; }

        /// <summary>
        /// Gets the attack power of this combatant.
        /// </summary>
        public abstract ushort AttackPower { get; }

        /// <summary>
        /// Gets the defense power of this combatant.
        /// </summary>
        public abstract ushort DefensePower { get; }

        /// <summary>
        /// Gets the armor rating of this combatant.
        /// </summary>
        public abstract ushort ArmorRating { get; }

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
        /// Clears the tracking store of damage taken from other combatants.
        /// </summary>
        public void ClearDamageTaken()
        {
            lock (this.damageTakenFromOthersLock)
            {
                this.damageTakenFromOthers.Clear();
            }
        }

        /// <summary>
        /// Tracks damage taken by a combatant.
        /// </summary>
        /// <param name="fromCombatantId">The combatant from which to track the damage.</param>
        /// <param name="damage">The value of the damage.</param>
        public void RecordDamageTaken(uint fromCombatantId, int damage)
        {
            if (fromCombatantId == 0 || damage < 0)
            {
                return;
            }

            lock (this.damageTakenFromOthersLock)
            {
                if (!this.damageTakenFromOthers.ContainsKey(fromCombatantId))
                {
                    this.damageTakenFromOthers[fromCombatantId] = 0u;
                }

                // Add to the Hostiles list so that this creature can choose to attack them.
                // TOOD: should we check for Friends here and avoid adding them?
                this.HostilesInView.Add(fromCombatantId);

                this.damageTakenFromOthers[fromCombatantId] = this.damageTakenFromOthers[fromCombatantId] + (uint)damage;
            }
        }

        /// <summary>
        /// Consumes combat credits to the combatant.
        /// </summary>
        /// <param name="creditType">The type of combat credits.</param>
        /// <param name="amount">The amount of credits.</param>
        public void ConsumeCredits(CombatCreditType creditType, byte amount)
        {
            switch (creditType)
            {
                case CombatCreditType.Attack:
                    var oldAttackCredits = this.AutoAttackCredits;

                    this.AutoAttackCredits = (byte)Math.Max(0, this.AutoAttackCredits - amount);

                    if (this.AutoAttackCredits != oldAttackCredits)
                    {
                        this.CombatCreditsConsumed?.Invoke(this, creditType, amount);
                    }

                    break;

                case CombatCreditType.Defense:
                    var oldDefenseCredits = this.AutoDefenseCredits;

                    this.AutoDefenseCredits = (byte)Math.Max(0, this.AutoDefenseCredits - amount);

                    if (this.AutoDefenseCredits != oldDefenseCredits)
                    {
                        this.CombatCreditsConsumed?.Invoke(this, creditType, amount);
                    }

                    break;
            }
        }

        /// <summary>
        /// Restores combat credits to the combatant.
        /// </summary>
        /// <param name="creditType">The type of combat credits.</param>
        /// <param name="amount">The amount of credits.</param>
        public void RestoreCredits(CombatCreditType creditType, byte amount)
        {
            switch (creditType)
            {
                case CombatCreditType.Attack:
                    this.AutoAttackCredits = (byte)Math.Min(this.AutoAttackMaximumCredits, this.AutoAttackCredits + amount);
                    break;

                case CombatCreditType.Defense:
                    this.AutoDefenseCredits = (byte)Math.Min(this.AutoDefenseMaximumCredits, this.AutoDefenseCredits + amount);
                    break;
            }
        }

        /// <summary>
        /// Sets the attack target of this combatant.
        /// </summary>
        /// <param name="otherCombatant">The other target combatant, if any.</param>
        public void SetAttackTarget(ICombatant otherCombatant)
        {
            if (otherCombatant != this.AutoAttackTarget)
            {
                var oldTarget = this.AutoAttackTarget;

                this.AutoAttackTarget = otherCombatant;

                if (this.ChaseMode == ChaseMode.Chase || otherCombatant == null)
                {
                    this.ChasingTarget = otherCombatant;
                }

                this.TargetChanged?.Invoke(this, oldTarget);
            }
        }

        /// <summary>
        /// Sets a <see cref="ICombatant"/> now in view for this combatant.
        /// </summary>
        /// <param name="otherCombatant">The other combatant, now in view.</param>
        public abstract void CombatantNowInView(ICombatant otherCombatant);

        /// <summary>
        /// Sets a <see cref="ICombatant"/> as no longer in view for this combatant.
        /// </summary>
        /// <param name="otherCombatant">The other combatant, now in view.</param>
        public abstract void CombatantNoLongerInView(ICombatant otherCombatant);

        /// <summary>
        /// Invokes the fight mode changed event.
        /// </summary>
        /// <param name="oldMode">The mode from which the combatant changed.</param>
        protected void InvokeFightModeChanged(FightMode oldMode)
        {
            this.FightModeChanged?.Invoke(this, oldMode);
        }

        /// <summary>
        /// Invokes the chase mode changed event.
        /// </summary>
        /// <param name="oldMode">The mode from which the combatant changed.</param>
        protected void InvokeChaseModeChanged(ChaseMode oldMode)
        {
            this.ChaseModeChanged?.Invoke(this, oldMode);
        }

        /// <summary>
        /// Invokes the combat started event.
        /// </summary>
        protected void InvokeCombatStarted()
        {
            this.CombatStarted?.Invoke(this);
        }

        /// <summary>
        /// Invokes the combat ended event.
        /// </summary>
        protected void InvokeCombatEnded()
        {
            this.CombatEnded?.Invoke(this);
        }
    }
}
