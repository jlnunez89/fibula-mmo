// -----------------------------------------------------------------
// <copyright file="BloodType.cs" company="2Dudes">
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
    /// Enumerates the possible blood type in the game.
    /// </summary>
    public enum BloodType : byte
    {
        /// <summary>
        /// Normal blood.
        /// </summary>
        Blood,

        /// <summary>
        /// Fire.
        /// </summary>
        Fire,

        /// <summary>
        /// Slime.
        /// </summary>
        Slime,

        /// <summary>
        /// Bones.
        /// </summary>
        Bones,
    }
}
