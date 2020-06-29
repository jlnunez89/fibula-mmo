// -----------------------------------------------------------------
// <copyright file="ICombatant.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Combat.Enumerations;
    using Fibula.Mechanics.Contracts.Delegates;
    using Fibula.Mechanics.Contracts.Structs;

    /// <summary>
    /// Interface for all creatures that can participate in combat.
    /// </summary>
    public interface ICombatant : ICreatureWithExhaustion
    {
        /// <summary>
        /// Event to call when the combatant's health changes.
        /// </summary>
        event OnHealthChange HealthChanged;

        ///// <summary>
        ///// Event to call when the attack target changes.
        ///// </summary>
        // event OnAttackTargetChange TargetChanged;

        ///// <summary>
        ///// Event to call when the fight mode changes.
        ///// </summary>
        // event FightModeChanged FightModeChanged;

        ///// <summary>
        ///// Event to call when the chase mode changes.
        ///// </summary>
        // event ChaseModeChanged ChaseModeChanged;

        ///// <summary>
        ///// Gets the set of creatures currently in view for this combatant.
        ///// </summary>
        // IEnumerable<uint> CreaturesInView { get; }

        ///// <summary>
        ///// Gets the set of ids of creatures that this combatant considers hostile, and tipically initiates combat against.
        ///// </summary>
        // ISet<uint> HostilesInView { get; }

        ///// <summary>
        ///// Gets the set of ids of creatures that this combatant considers neutral.
        ///// </summary>
        // ISet<uint> NeutralsInView { get; }

        ///// <summary>
        ///// Gets the set of ids of creatures that this combatant considers friendly, and tipically treats favorably.
        ///// </summary>
        // ISet<uint> FriendlyInView { get; }

        /// <summary>
        /// Gets the current target combatant, if any.
        /// </summary>
        ICombatant AutoAttackTarget { get; }

        /// <summary>
        /// Gets the current target being chased, if any.
        /// </summary>
        ICombatant ChasingTarget { get; }

        /// <summary>
        /// Gets the range that the auto attack has.
        /// </summary>
        byte AutoAttackRange { get; }

        /// <summary>
        /// Gets the number of attack credits available.
        /// </summary>
        int AutoAttackCredits { get; }

        /// <summary>
        /// Gets the number of maximum attack credits.
        /// </summary>
        ushort AutoAttackMaximumCredits { get; }

        /// <summary>
        /// Gets a metric of how fast a combatant can earn an attack credit per combat round.
        /// </summary>
        decimal AttackSpeed { get; }

        /// <summary>
        /// Gets the number of auto defense credits available.
        /// </summary>
        int AutoDefenseCredits { get; }

        /// <summary>
        /// Gets the number of maximum defense credits.
        /// </summary>
        ushort AutoDefenseMaximumCredits { get; }

        /// <summary>
        /// Gets a metric of how fast a combatant can earn a defense credit per combat round.
        /// </summary>
        decimal DefenseSpeed { get; }

        ///// <summary>
        ///// Gets the attack power of this combatant.
        ///// </summary>
        // ushort AttackPower { get; }

        ///// <summary>
        ///// Gets the defense power of this combatant.
        ///// </summary>
        // ushort DefensePower { get; }

        ///// <summary>
        ///// Gets the armor rating of this combatant.
        ///// </summary>
        // ushort ArmorRating { get; }

        /// <summary>
        /// Gets or sets the current auto attack operation that this combatant has pending, if any.
        /// </summary>
        IOperation PendingAutoAttackOperation { get; set; }

        /// <summary>
        /// Gets the distribution of damage taken by any combatant that has attacked this combatant while the current combat is active.
        /// </summary>
        IEnumerable<(uint, uint)> DamageTakenDistribution { get; }

        /// <summary>
        /// Gets the collection of ids of attackers of this combatant.
        /// </summary>
        IEnumerable<uint> AttackedBy { get; }

        /// <summary>
        /// Gets or sets the fight mode selected by this combatant.
        /// </summary>
        FightMode FightMode { get; set; }

        /// <summary>
        /// Gets or sets the chase mode selected by this combatant.
        /// </summary>
        ChaseMode ChaseMode { get; set; }

        /// <summary>
        /// Sets the attack target of this combatant.
        /// </summary>
        /// <param name="otherCombatant">The other target combatant, if any.</param>
        /// <returns>True if the target was actually changed, false otherwise.</returns>
        bool SetAttackTarget(ICombatant otherCombatant);

        /// <summary>
        /// Consumes combat credits to the combatant.
        /// </summary>
        /// <param name="creditType">The type of combat credits to consume.</param>
        /// <param name="amount">The amount of credits to consume.</param>
        void ConsumeCredits(CombatCreditType creditType, byte amount);

        /// <summary>
        /// Restores combat credits to the combatant.
        /// </summary>
        /// <param name="creditType">The type of combat credits to restore.</param>
        /// <param name="amount">The amount of credits to restore.</param>
        void RestoreCredits(CombatCreditType creditType, byte amount);

        /// <summary>
        /// Increases the attack speed of this combatant.
        /// </summary>
        /// <param name="increaseAmount">The amount by which to increase.</param>
        void IncreaseAttackSpeed(decimal increaseAmount);

        /// <summary>
        /// Increases the defense speed of this combatant.
        /// </summary>
        /// <param name="increaseAmount">The amount by which to increase.</param>
        void IncreaseDefenseSpeed(decimal increaseAmount);

        /// <summary>
        /// Decreases the attack speed of this combatant.
        /// </summary>
        /// <param name="decreaseAmount">The amount by which to decrease.</param>
        void DecreaseAttackSpeed(decimal decreaseAmount);

        /// <summary>
        /// Decreases the defense speed of this combatant.
        /// </summary>
        /// <param name="decreaseAmount">The amount by which to decrease.</param>
        void DecreaseDefenseSpeed(decimal decreaseAmount);

        /// <summary>
        /// Applies damage to the combatant, which is expected to apply reductions and protections.
        /// </summary>
        /// <param name="damageInfo">The information of the damage to make, without reductions.</param>
        /// <param name="fromCombatantId">The combatant from which to track the damage, if any.</param>
        /// <returns>The information about the damage actually done.</returns>
        DamageInfo ApplyDamage(DamageInfo damageInfo, uint fromCombatantId = 0);

        ///// <summary>
        ///// Clears the tracking store of damage taken from other combatants.
        ///// </summary>
        // void ClearDamageTaken();

        ///// <summary>
        ///// Sets a <see cref="ICombatant"/> now in view for this combatant.
        ///// </summary>
        ///// <param name="otherCombatant">The other combatant, now in view.</param>
        // void CombatantNowInView(ICombatant otherCombatant);

        ///// <summary>
        ///// Sets a <see cref="ICombatant"/> as no longer in view for this combatant.
        ///// </summary>
        ///// <param name="otherCombatant">The other combatant, now in view.</param>
        // void CombatantNoLongerInView(ICombatant otherCombatant);
    }
}
