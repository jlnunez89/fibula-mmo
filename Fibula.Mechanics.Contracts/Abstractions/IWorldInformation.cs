// -----------------------------------------------------------------
// <copyright file="IWorldInformation.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Interface for the world information.
    /// </summary>
    public interface IWorldInformation
    {
        /// <summary>
        /// Gets the game world's light color.
        /// </summary>
        byte LightColor { get; }

        /// <summary>
        /// Gets the game world's light level.
        /// </summary>
        byte LightLevel { get; }

        /// <summary>
        /// Gets the game world's state.
        /// </summary>
        WorldState Status { get; }
    }
}
