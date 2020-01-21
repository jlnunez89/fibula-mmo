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
    using OpenTibia.Server.Contracts.Delegates;

    /// <summary>
    /// Interface for all creatures that can participate in combat.
    /// </summary>
    public interface ICombatant : ICreature
    {
        /// <summary>
        /// Event to call when the attack target changes.
        /// </summary>
        event OnAttackTargetChange OnTargetChanged;

        /// <summary>
        /// Gets the id of the auto attack target.
        /// </summary>
        uint AutoAttackTargetId { get; }

        /// <summary>
        /// Gets the range that the auto attack has.
        /// </summary>
        byte AutoAttackRange { get; }

        /// <summary>
        /// Gets the number of attack credits available.
        /// </summary>
        byte AutoAttackCredits { get; }

        /// <summary>
        /// Gets the number of auto defense credits.
        /// </summary>
        byte AutoDefenseCredits { get; }

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
        /// Sets the attack target of this combatant.
        /// </summary>
        /// <param name="targetId">The id of the new target, if any.</param>
        void SetAttackTarget(uint targetId);
    }
}
