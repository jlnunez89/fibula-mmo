// -----------------------------------------------------------------
// <copyright file="ICombatant.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System.Collections.Generic;
    using OpenTibia.Server.Contracts.Delegates;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for all creatures that can participate in combat.
    /// </summary>
    public interface ICombatant : ICreature
    {
        /// <summary>
        /// Event to call when the attack target changes.
        /// </summary>
        event OnAttackTargetChange TargetChanged;

        /// <summary>
        /// Event to call when the fight mode changes.
        /// </summary>
        event FightModeChanged FightModeChanged;

        /// <summary>
        /// Event to call when the chase mode changes.
        /// </summary>
        event ChaseModeChanged ChaseModeChanged;

        /// <summary>
        /// Event to call when a combat credit is consumed.
        /// </summary>
        event CombatCreditConsumed CombatCreditsConsumed;

        /// <summary>
        /// Gets the auto attack target combatant.
        /// </summary>
        ICombatant AutoAttackTarget { get; }

        /// <summary>
        /// Gets the range that the auto attack has.
        /// </summary>
        byte AutoAttackRange { get; }

        /// <summary>
        /// Gets the number of attack credits available.
        /// </summary>
        byte AutoAttackCredits { get; }

        /// <summary>
        /// Gets the number of maximum attack credits.
        /// </summary>
        byte AutoAttackMaximumCredits { get; }

        /// <summary>
        /// Gets the number of auto defense credits available.
        /// </summary>
        byte AutoDefenseCredits { get; }

        /// <summary>
        /// Gets the number of maximum defense credits.
        /// </summary>
        byte AutoDefenseMaximumCredits { get; }

        /// <summary>
        /// Gets a metric of how fast an Actor can earn a new AutoAttack credit per second.
        /// </summary>
        decimal BaseAttackSpeed { get; }

        /// <summary>
        /// Gets a metric of how fast an Actor can earn a new AutoDefense credit per second.
        /// </summary>
        decimal BaseDefenseSpeed { get; }

        /// <summary>
        /// Gets the attack power of this combatant.
        /// </summary>
        ushort AttackPower { get; }

        /// <summary>
        /// Gets the defense power of this combatant.
        /// </summary>
        ushort DefensePower { get; }

        /// <summary>
        /// Gets the armor rating of this combatant.
        /// </summary>
        ushort ArmorRating { get; }

        /// <summary>
        /// Gets the distribution of damage taken by any combatant that has attacked this combatant while the current combat is active.
        /// </summary>
        IEnumerable<(uint, uint)> DamageTakenDistribution { get; }

        /// <summary>
        /// Gets the collection of ids of attackers of this combatant.
        /// </summary>
        IEnumerable<uint> AttackedBy { get; }

        /// <summary>
        /// Gets or sets the chase mode selected by this combatant.
        /// </summary>
        ChaseMode ChaseMode { get; set; }

        /// <summary>
        /// Gets or sets the fight mode selected by this combatant.
        /// </summary>
        FightMode FightMode { get; set; }

        /// <summary>
        /// Sets the attack target of this combatant.
        /// </summary>
        /// <param name="otherCombatant">The other target combatant, if any.</param>
        void SetAttackTarget(ICombatant otherCombatant);

        /// <summary>
        /// Consumes combat credits to the combatant.
        /// </summary>
        /// <param name="creditType">The type of combat credits.</param>
        /// <param name="amount">The amount of credits.</param>
        void ConsumeCredits(CombatCreditType creditType, byte amount);

        /// <summary>
        /// Restores combat credits to the combatant.
        /// </summary>
        /// <param name="creditType">The type of combat credits.</param>
        /// <param name="amount">The amount of credits.</param>
        void RestoreCredits(CombatCreditType creditType, byte amount);

        /// <summary>
        /// Tracks damage taken by a combatant.
        /// </summary>
        /// <param name="fromCombatantId">The combatant from which to track the damage.</param>
        /// <param name="damage">The value of the damage.</param>
        void RecordDamageTaken(uint fromCombatantId, int damage);

        /// <summary>
        /// Clears the tracking store of damage taken from other combatants.
        /// </summary>
        void ClearDamageTaken();
    }
}
