// -----------------------------------------------------------------
// <copyright file="IWorldInformation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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