// -----------------------------------------------------------------
// <copyright file="AvoidDamageType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
