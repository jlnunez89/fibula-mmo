// -----------------------------------------------------------------
// <copyright file="Direction.cs" company="2Dudes">
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
    /// Enumeration of the directions in the game.
    /// </summary>
    public enum Direction : byte
    {
        /// <summary>
        /// North.
        /// </summary>
        North,

        /// <summary>
        /// East.
        /// </summary>
        East,

        /// <summary>
        /// South.
        /// </summary>
        South,

        /// <summary>
        /// West.
        /// </summary>
        West,

        /// <summary>
        /// NorthEast.
        /// </summary>
        NorthEast,

        /// <summary>
        /// SouthEast.
        /// </summary>
        SouthEast,

        /// <summary>
        /// NorthWest.
        /// </summary>
        NorthWest,

        /// <summary>
        /// SouthWest.
        /// </summary>
        SouthWest,
    }
}
