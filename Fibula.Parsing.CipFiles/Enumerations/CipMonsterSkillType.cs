// -----------------------------------------------------------------
// <copyright file="CipMonsterSkillType.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Parsing.CipFiles.Enumerations
{
    /// <summary>
    /// Enumerates the possible monster skill types from CIP files.
    /// </summary>
    public enum CipMonsterSkillType : byte
    {
        /// <summary>
        /// The hitpoints of the monster.
        /// </summary>
        Hitpoints,

        /// <summary>
        /// The speed strength of the monster.
        /// </summary>
        GoStrength,

        /// <summary>
        /// The maximum capacity of the monster.
        /// </summary>
        CarryStrength,

        /// <summary>
        /// The base fighting skill of the monster (unarmed).
        /// </summary>
        FistFighting,
    }
}
