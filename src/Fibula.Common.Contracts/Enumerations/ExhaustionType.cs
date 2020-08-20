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

namespace Fibula.Common.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the possible forms of the exhaustion condition.
    /// </summary>
    public enum ExhaustionType : byte
    {
        /// <summary>
        /// Exhausted to use items or otherwise perform actions.
        /// </summary>
        Action = 1 << 0,

        /// <summary>
        /// Exhausted to fight physically.
        /// </summary>
        Combat = 1 << 1,

        /// <summary>
        /// Exhausted to cast magic.
        /// </summary>
        Magic = 1 << 2,

        /// <summary>
        /// Exhausted to move.
        /// </summary>
        Movement = 1 << 3,
    }
}
