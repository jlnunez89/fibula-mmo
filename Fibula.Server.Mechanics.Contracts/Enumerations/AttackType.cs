// -----------------------------------------------------------------
// <copyright file="AttackType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Mechanics.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the different attack types.
    /// </summary>
    public enum AttackType : byte
    {
        /// <summary>
        /// Physical attack type.
        /// </summary>
        Physical,

        /// <summary>
        /// Poison based attack type.
        /// </summary>
        Posion,

        /// <summary>
        /// Fire based attack type.
        /// </summary>
        Fire,

        /// <summary>
        /// Electricity based attack type.
        /// </summary>
        Electric,

        /// <summary>
        /// Draining attack type.
        /// </summary>
        Drain,
    }
}
