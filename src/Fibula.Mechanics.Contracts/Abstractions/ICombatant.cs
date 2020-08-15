// -----------------------------------------------------------------
// <copyright file="ICombatant.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Delegates;
    using Fibula.Mechanics.Contracts.Structs;

    /// <summary>
    /// Interface for all creatures that can participate in combat.
    /// </summary>
    public interface ICombatant : ICreatureWithSkills, ICreatureThatSensesOthers
    {
        /// <summary>
        /// Event to call when the combatant dies.
        /// </summary>
        event OnDeath Death;

        /// <summary>
        /// Event to call when the attack target changes.
        /// </summary>
        event OnAttackTargetChanged AttackTargetChanged;

        /// <summary>
        /// Event to call when the chase target changes.
        /// </summary>
        event OnFollowTargetChanged FollowTargetChanged;

        /// <summary>
        /// Gets the current target being chased, if any.
        /// </summary>
        ICreature ChaseTarget { get; }

        /// <summary>
        /// Gets the current target combatant, if any.
        /// </summary>
        ICombatant AutoAttackTarget { get; }

        /// <summary>
        /// Gets the range that the auto attack has.
        /// </summary>
        byte AutoAttackRange { get; }

        /// <summary>
        /// Gets a metric of how fast a combatant can earn an attack credit per combat round.
        /// </summary>
        decimal AttackSpeed { get; }

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
        /// Gets the distribution of damage taken by any combatant that has attacked this combatant while the current combat is active.
        /// </summary>
        IEnumerable<(uint, uint)> DamageTakenInSession { get; }

        /// <summary>
        /// Gets the collection of combatants currently attacking this combatant.
        /// </summary>
        IEnumerable<ICombatant> AttackedBy { get; }

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
        /// Sets the chasing target of this combatant.
        /// </summary>
        /// <param name="target">The target to chase, if any.</param>
        /// <returns>True if the target was actually changed, false otherwise.</returns>
        bool SetFollowTarget(ICreature target);

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

        /// <summary>
        /// Adds a combatant to the combat list of this one.
        /// </summary>
        /// <param name="otherCombatant">The combatant to add to the list.</param>
        void AddToCombatList(ICombatant otherCombatant);

        /// <summary>
        /// Removes a combatant from the combat list of this one.
        /// </summary>
        /// <param name="otherCombatant">The combatant to remove from the list.</param>
        void RemoveFromCombatList(ICombatant otherCombatant);

        /// <summary>
        /// Sets this combatant as being attacked by another.
        /// </summary>
        /// <param name="combatant">The combatant attacking this one, if any.</param>
        void SetAttackedBy(ICombatant combatant);

        /// <summary>
        /// Unsets this combatant as being attacked by another.
        /// </summary>
        /// <param name="combatant">The combatant no longer attacking this one, if any.</param>
        void UnsetAttackedBy(ICombatant combatant);
    }
}
