// -----------------------------------------------------------------
// <copyright file="ICreatureManager.cs" company="2Dudes">
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
    /// <summary>
    /// Interface for a creature manager.
    /// </summary>
    public interface ICreatureManager : ICreatureFinder
    {
        /// <summary>
        /// Gets the count of players registered in the manager.
        /// </summary>
        int PlayerCount { get; }

        /// <summary>
        /// Registers a new creature to the manager.
        /// </summary>
        /// <param name="creature">The creature to register.</param>
        void RegisterCreature(ICreature creature);

        /// <summary>
        /// Unregisters a creature from the manager.
        /// </summary>
        /// <param name="creature">The creature to unregister.</param>
        void UnregisterCreature(ICreature creature);
    }
}
