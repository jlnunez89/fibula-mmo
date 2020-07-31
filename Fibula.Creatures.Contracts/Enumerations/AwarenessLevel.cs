// -----------------------------------------------------------------
// <copyright file="AwarenessLevel.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the different levels of creature awareness.
    /// </summary>
    public enum AwarenessLevel
    {
        /// <summary>
        /// A creature has been sensed but not yet seen.
        /// </summary>
        Sensed,

        /// <summary>
        /// A creature has been seen.
        /// </summary>
        Seen,
    }
}
