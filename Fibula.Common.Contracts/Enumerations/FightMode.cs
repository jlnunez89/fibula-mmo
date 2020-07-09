// -----------------------------------------------------------------
// <copyright file="FightMode.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Enumerations
{
    /// <summary>
    /// Enumeration of the possible fighting modes.
    /// </summary>
    public enum FightMode : byte
    {
        /// <summary>
        /// Full offensive attack.
        /// Focuses on dealing damage but leaves the attacker open to receive more damage, too.
        /// </summary>
        FullAttack = 0x01,

        /// <summary>
        /// A balanced stance.
        /// </summary>
        Balanced = 0x02,

        /// <summary>
        /// Least aggresive stance.
        /// Focuses on blocking attacks instead of dealing damage.
        /// </summary>
        FullDefense = 0x03,
    }
}
