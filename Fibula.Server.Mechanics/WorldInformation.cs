// -----------------------------------------------------------------
// <copyright file="WorldInformation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Mechanics
{
    using Fibula.Server.Mechanics.Contracts.Abstractions;
    using Fibula.Server.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Class that implements an <see cref="IWorldInformation"/> model.
    /// </summary>
    public class WorldInformation : IWorldInformation
    {
        /// <summary>
        /// Gets or sets the game world's light color.
        /// </summary>
        public byte LightColor { get; set; }

        /// <summary>
        /// Gets or sets the game world's light level.
        /// </summary>
        public byte LightLevel { get; set; }

        /// <summary>
        /// Gets or sets the game world's state.
        /// </summary>
        public WorldState Status { get; set; }
    }
}