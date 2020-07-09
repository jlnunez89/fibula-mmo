// -----------------------------------------------------------------
// <copyright file="SquareColor.cs" company="2Dudes">
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
    /// Enumerates the possible colors of the creature square.
    /// </summary>
    public enum SquareColor : byte
    {
        /// <summary>
        /// A pointer to the first valid value in this enumeration.
        /// </summary>
        First = Black,

        /// <summary>
        /// A black square.
        /// </summary>
        Black = 0x00,

        /// <summary>
        /// A white square.
        /// </summary>
        White = byte.MaxValue,

        /// <summary>
        /// A pointer to the last valid value in this enumeration.
        /// </summary>
        Last = White,
    }
}
