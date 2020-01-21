// -----------------------------------------------------------------
// <copyright file="TextColor.cs" company="2Dudes">
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
    /// Enumerates the possible colors of text.
    /// </summary>
    public enum TextColor : byte
    {
        /// <summary>
        /// Blue.
        /// </summary>
        Blue = 5,

        /// <summary>
        /// Green.
        /// </summary>
        Green = 30,

        /// <summary>
        /// Light blue.
        /// </summary>
        LightBlue = 35,

        /// <summary>
        /// Crystal.
        /// </summary>
        Crystal = 65,

        /// <summary>
        /// Purple.
        /// </summary>
        Purple = 83,

        /// <summary>
        /// Platinum.
        /// </summary>
        Platinum = 89,

        /// <summary>
        /// Light gray.
        /// </summary>
        LightGrey = 129,

        /// <summary>
        /// Dark red.
        /// </summary>
        DarkRed = 144,

        /// <summary>
        /// Red.
        /// </summary>
        Red = 180,

        /// <summary>
        /// Orange.
        /// </summary>
        Orange = 198,

        /// <summary>
        /// Golden.
        /// </summary>
        Gold = 210,

        /// <summary>
        /// White.
        /// </summary>
        White = 215,

        /// <summary>
        /// No color.
        /// </summary>
        None = 255,
    }
}
