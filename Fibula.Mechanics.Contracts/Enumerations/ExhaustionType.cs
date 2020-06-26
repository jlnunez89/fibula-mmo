// -----------------------------------------------------------------
// <copyright file="ExhaustionType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the possible sources of creature exhaustion.
    /// </summary>
    public enum ExhaustionType
    {
        /// <summary>
        /// No exhaustion.
        /// </summary>
        None,

        /// <summary>
        /// Moving around.
        /// </summary>
        Movement,

        /// <summary>
        /// Fighting (including defending).
        /// </summary>
        PhysicalCombat,

        /// <summary>
        /// Casting spells or using runes.
        /// </summary>
        MentalCombat,

        /// <summary>
        /// Using items or performing actions.
        /// </summary>
        Action,

        /// <summary>
        /// Talking in the public chat channel.
        /// </summary>
        Speech,
    }
}
