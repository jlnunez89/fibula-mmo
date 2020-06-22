// -----------------------------------------------------------------
// <copyright file="LightLevels.cs" company="2Dudes">
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
    /// Enumeration of the common light levels.
    /// </summary>
    public enum LightLevels : byte
    {
        /// <summary>
        /// No light.
        /// </summary>
        None = 0,

        /// <summary>
        /// The level emitted by a brand-new torch.
        /// </summary>
        Torch = 7,

        /// <summary>
        /// The level considered as full.
        /// </summary>
        Full = 27,

        /// <summary>
        /// Daylight level.
        /// </summary>
        World = 255,
    }
}
