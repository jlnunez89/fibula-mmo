// -----------------------------------------------------------------
// <copyright file="ExhaustionType.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
