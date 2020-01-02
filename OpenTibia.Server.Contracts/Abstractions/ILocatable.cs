// -----------------------------------------------------------------
// <copyright file="ILocatable.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Interface for all entities with a location.
    /// </summary>
    public interface ILocatable
    {
        /// <summary>
        /// Gets this cylinder's location.
        /// </summary>
        Location Location { get; }
    }
}