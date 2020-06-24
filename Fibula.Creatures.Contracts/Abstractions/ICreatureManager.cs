// -----------------------------------------------------------------
// <copyright file="ICreatureManager.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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