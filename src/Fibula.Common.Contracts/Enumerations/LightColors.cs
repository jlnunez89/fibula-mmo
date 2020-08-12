// -----------------------------------------------------------------
// <copyright file="LightColors.cs" company="2Dudes">
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
    /// Enumeration of the possible light colors.
    /// </summary>
    public enum LightColors : byte
    {
        /// <summary>
        /// No color.
        /// </summary>
        None = 0,

        /// <summary>
        /// The default color, which is <see cref="Orange"/>.
        /// </summary>
        Default = Orange,

        /// <summary>
        /// Orange color.
        /// </summary>
        Orange = 206,

        /// <summary>
        /// White color.
        /// </summary>
        White = 215,
    }
}
