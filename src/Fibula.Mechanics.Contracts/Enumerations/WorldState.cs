// -----------------------------------------------------------------
// <copyright file="WorldState.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Enumerations
{
    /// <summary>
    /// Enumeration of the possible world states.
    /// </summary>
    public enum WorldState : byte
    {
        /// <summary>
        /// Represents a world in loading state.
        /// </summary>
        Loading,

        /// <summary>
        /// The normal, open public state.
        /// </summary>
        Open,

        /// <summary>
        /// Testing or closed beta state.
        /// </summary>
        Closed,
    }
}
