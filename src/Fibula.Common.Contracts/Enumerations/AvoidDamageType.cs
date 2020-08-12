// -----------------------------------------------------------------
// <copyright file="AvoidDamageType.cs" company="2Dudes">
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
    /// Enumerates the possible damage types to avoid.
    /// </summary>
    public enum AvoidDamageType : byte
    {
        /// <summary>
        /// Avoids special.
        /// </summary>
        Special = 0,

        /// <summary>
        /// Avoid poison.
        /// </summary>
        Poison = 1 << 1,

        /// <summary>
        /// Avoid fire.
        /// </summary>
        Fire = 1 << 2,

        /// <summary>
        /// Avoid energy.
        /// </summary>
        Energy = 1 << 3,

        /// <summary>
        /// Avoid all types.
        /// </summary>
        All = Poison | Fire | Energy,
    }
}
