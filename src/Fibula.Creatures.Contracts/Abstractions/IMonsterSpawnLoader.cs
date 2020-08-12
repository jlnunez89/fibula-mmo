// -----------------------------------------------------------------
// <copyright file="IMonsterSpawnLoader.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Creatures.Contracts.Structs;

    /// <summary>
    /// Interface for an <see cref="IMonsterSpawnLoader"/> loader.
    /// </summary>
    public interface IMonsterSpawnLoader
    {
        /// <summary>
        /// Attempts to load the monster spawns.
        /// </summary>
        /// <returns>The collection of loaded monster spawns.</returns>
        IEnumerable<Spawn> LoadSpawns();
    }
}
