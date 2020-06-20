// -----------------------------------------------------------------
// <copyright file="LightColors.cs" company="2Dudes">
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
