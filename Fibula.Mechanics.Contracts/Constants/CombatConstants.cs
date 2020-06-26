// -----------------------------------------------------------------
// <copyright file="CombatConstants.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
        public const decimal DefaultDefenseSpeed = 2.0m;

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
    }
}
