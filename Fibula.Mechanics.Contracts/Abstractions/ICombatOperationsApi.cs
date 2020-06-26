// -----------------------------------------------------------------
// <copyright file="ICombatOperationsApi.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Enumerations;

    /// <summary>
    /// Interface for the API of combat operations.
    /// </summary>
    public interface ICombatOperationsApi
    {
        // void OnCombatantCombatStarted(ICombatant combatant);

        // void OnCombatantCombatEnded(ICombatant combatant);

        // void OnCombatCreditsConsumed(ICombatant combatant, CombatCreditType creditType, byte amount);

        // void OnCombatantTargetChanged(ICombatant combatant, ICombatant oldTarget);

        // void OnCombatantChaseModeChanged(ICombatant combatant, ChaseMode oldMode);

        /// <summary>
        /// Re-sets the combat target of the attacker and it's (possibly new) target.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="target">The target.</param>
        void ResetCombatTarget(ICombatant attacker, ICombatant target);

        /// <summary>
        /// Changes the fight, chase and safety modes of a creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature.</param>
        /// <param name="fightMode">The fight mode to change to.</param>
        /// <param name="chaseMode">The chase mode to change to.</param>
        /// <param name="safeModeOn">A value indicating whether the attack safety lock is on.</param>
        void CreatureChangeModes(uint creatureId, FightMode fightMode, ChaseMode chaseMode, bool safeModeOn);
    }
}