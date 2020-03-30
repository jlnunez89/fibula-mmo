// -----------------------------------------------------------------
// <copyright file="IGame.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using Microsoft.Extensions.Hosting;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for the game instance.
    /// </summary>
    public interface IGame : IHostedService
    {
        /// <summary>
        /// Gets the game world's light color.
        /// </summary>
        byte WorldLightColor { get; }

        /// <summary>
        /// Gets the game world's light level.
        /// </summary>
        byte WorldLightLevel { get; }

        /// <summary>
        /// Gets the game world's state.
        /// </summary>
        WorldState Status { get; }
    }
}