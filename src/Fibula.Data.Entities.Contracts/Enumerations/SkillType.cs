// -----------------------------------------------------------------
// <copyright file="SkillType.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Entities.Contracts.Enumerations
{
    /// <summary>
    /// Enumeration of the possible skills in the game.
    /// </summary>
    public enum SkillType : byte
    {
        /// <summary>
        /// The experience level.
        /// </summary>
        Experience,

        /// <summary>
        /// The magic level.
        /// </summary>
        Magic,

        /// <summary>
        /// The bare-hands or non-weapon fighting level.
        /// </summary>
        NoWeapon,

        /// <summary>
        /// The axe fighting level.
        /// </summary>
        Axe,

        /// <summary>
        /// The club fighting level.
        /// </summary>
        Club,

        /// <summary>
        /// The sword fighting level.
        /// </summary>
        Sword,

        /// <summary>
        /// The shielding level.
        /// </summary>
        Shield,

        /// <summary>
        /// The ranged fighting level.
        /// </summary>
        Ranged,

        /// <summary>
        /// The fishing level.
        /// </summary>
        Fishing,
    }
}
