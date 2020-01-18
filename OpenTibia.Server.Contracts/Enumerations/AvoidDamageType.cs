// -----------------------------------------------------------------
// <copyright file="AvoidDamageType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the possible blood types.
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
