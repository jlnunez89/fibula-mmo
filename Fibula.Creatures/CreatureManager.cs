// -----------------------------------------------------------------
// <copyright file="CreatureManager.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents a creature manager.
    /// </summary>
    public class CreatureManager : ICreatureManager
    {
        /// <summary>
        /// Holds the <see cref="ConcurrentDictionary{TKey,TValue}"/> of all <see cref="ICreature"/>s in the game, in which the Key is the <see cref="ICreature.Id"/>.
        /// </summary>
        private readonly ConcurrentDictionary<uint, ICreature> creatureMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureManager"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger to use.</param>
        public CreatureManager(ILogger logger)
        {
            logger.ThrowIfNull(nameof(logger));

            this.creatureMap = new ConcurrentDictionary<uint, ICreature>();

            this.Logger = logger.ForContext<CreatureManager>();
        }

        /// <summary>
        /// Gets a reference to the logger.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Registers a new creature to the manager.
        /// </summary>
        /// <param name="creature">The creature to register.</param>
        public void RegisterCreature(ICreature creature)
        {
            creature.ThrowIfNull(nameof(creature));

            if (!this.creatureMap.TryAdd(creature.Id, creature))
            {
                this.Logger.Warning($"Failed to add {creature.Name} ({creature.Id}) to the creatue manager.");
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
            this.creatureMap.TryGetValue(creatureId, out ICreature creature);

            return creature;
        }

        /// <summary>
        /// Looks for a single player with the id.
        /// </summary>
        /// <param name="playerId">The player id for which to look.</param>
        /// <returns>The player instance, if found, and null otherwise.</returns>
        public IPlayer FindPlayerById(uint playerId)
        {
            var creature = this.FindCreatureById(playerId);

            return creature as IPlayer;
        }

        /// <summary>
        /// Gets all creatures known to this manager.
        /// </summary>
        /// <returns>A collection of creature instances.</returns>
        public IEnumerable<ICreature> FindAllCreatures()
        {
            return this.creatureMap.Values;
        }

        /// <summary>
        /// Gets all players known to this manager.
        /// </summary>
        /// <returns>A collection of creature instances.</returns>
        public IEnumerable<IPlayer> FindAllPlayers()
        {
            return this.creatureMap.Values.Select(c => c as IPlayer).Where(p => p != null);
        }
    }
}
