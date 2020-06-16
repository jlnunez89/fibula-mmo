// -----------------------------------------------------------------
// <copyright file="SquareColor.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the possible colors of the creaure square.
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
        White = 0xFF,

        /// <summary>
        /// A pointer to the last valid value in this enumeration.
        /// </summary>
        Last = White,
    }
}
