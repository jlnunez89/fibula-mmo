// -----------------------------------------------------------------
// <copyright file="ChaseMode.cs" company="2Dudes">
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
    /// Enumeration of the possible chasing modes.
    /// </summary>
    public enum ChaseMode : byte
    {
        /// <summary>
        /// Does not chase the target.
        /// </summary>
        Stand,

        /// <summary>
        /// Chases the target closely.
        /// </summary>
        Chase,

        /// <summary>
        /// Maintains a constant distance to the target.
        /// </summary>
        KeepDistance,
    }
}
