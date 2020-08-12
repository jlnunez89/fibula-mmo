// -----------------------------------------------------------------
// <copyright file="LightLevels.cs" company="2Dudes">
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
