// -----------------------------------------------------------------
// <copyright file="CombatConstants.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Constants
{
    /// <summary>
    /// Static class that contains constants regarding combat.
    /// </summary>
    public static class CombatConstants
    {
        /// <summary>
        /// The maximum allowed combat speed in the game for combatants.
        /// </summary>
        public const decimal MaximumCombatSpeed = 5.0m;

        /// <summary>
        /// The minimum allowed combat speed in the game for combatants.
        /// </summary>
        public const decimal MinimumCombatSpeed = 0.2m;

        /// <summary>
        /// The default attack speed of a combatant.
        /// </summary>
        public const decimal DefaultAttackSpeed = 1.0m;

        /// <summary>
        /// The default defense speed of a combatant.
        /// </summary>
        public const decimal DefaultDefenseSpeed = 1.0m;

        /// <summary>
        /// The default maximum attack credits that a combatant has.
        /// </summary>
        public const ushort DefaultMaximumAttackCredits = 1;

        /// <summary>
        /// The default maximum defense credits that a combatant has.
        /// </summary>
        public const ushort DefaultMaximumDefenseCredits = 2;

        /// <summary>
        /// The default combat round time in milliseconds.
        /// </summary>
        public const int DefaultCombatRoundTimeInMs = 2000;

        /// <summary>
        /// The default in fight duration time in milliseconds.
        /// </summary>
        public const int DefaultInFightTimeInMs = 30000;
    }
}
