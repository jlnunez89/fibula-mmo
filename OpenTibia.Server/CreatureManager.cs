// -----------------------------------------------------------------
// <copyright file="CreatureManager.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a creature manager.
    /// </summary>
    public class CreatureManager : ICreatureManager
    {
        /// <summary>
        /// Holds the <see cref="ConcurrentDictionary{TKey,TValue}"/> of all <see cref="Creature"/>s in the game, in which the Key is the <see cref="Creature.Id"/>.
        /// </summary>
        private readonly ConcurrentDictionary<uint, ICreature> creatureMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureManager"/> class.
        /// </summary>
        public CreatureManager()
        {
            this.creatureMap = new ConcurrentDictionary<uint, ICreature>();
        }

        /// <summary>
        /// Registers a new creature to the manager.
        /// </summary>
        /// <param name="creature">The creature to register.</param>
        public void RegisterCreature(ICreature creature)
        {
            creature.ThrowIfNull(nameof(creature));

            if (!this.creatureMap.TryAdd(creature.Id, creature))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to add {creature.Name} ({creature.Id}) to the creatue manager.");
            }
        }

        /// <summary>
        /// Unregisters a creature from the manager.
        /// </summary>
        /// <param name="creature">The creature to unregister.</param>
        public void UnregisterCreature(ICreature creature)
        {
            creature.ThrowIfNull(nameof(creature));

            this.creatureMap.TryRemove(creature.Id, out _);
        }

        /// <summary>
        /// Looks for a single creature with the id.
        /// </summary>
        /// <param name="creatureId">The creature id for which to look.</param>
        /// <returns>The creature instance, if found, and null otherwise.</returns>
        public ICreature FindCreatureById(uint creatureId)
        {
            try
            {
                return this.creatureMap[creatureId];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all creatures known to this manager.
        /// </summary>
        /// <returns>A collection of creature instances.</returns>
        public IEnumerable<ICreature> FindAllCreatures()
        {
            return this.creatureMap.Values;
        }
    }
}
