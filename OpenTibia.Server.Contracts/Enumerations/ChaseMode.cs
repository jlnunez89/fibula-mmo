// -----------------------------------------------------------------
// <copyright file="ChaseMode.cs" company="2Dudes">
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
    /// Enumeration of the possible chasing modes.
    /// </summary>
    public enum ChaseMode : byte
    {
        /// <summary>
        /// Does not chase the target.
        /// </summary>
        Stand = 0x00,

        /// <summary>
        /// Chases the target closely.
        /// </summary>
        Chase = 0x01,

        /// <summary>
        /// Maintains a constant distance to the target.
        /// </summary>
        KeepDistance = 0x02,
    }
}
