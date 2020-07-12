// -----------------------------------------------------------------
// <copyright file="ElevatedOperationContext.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents an elevated context for operations.
    /// </summary>
    public class ElevatedOperationContext : OperationContext, IElevatedOperationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatedOperationContext"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="mapDescriptor">A reference to the map descriptor in use.</param>
        /// <param name="map">A reference to the map.</param>
        /// <param name="creatureManager">A reference to the creature manager in use.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="creatureFactory">A reference to the creature factory in use.</param>
        /// <param name="containerManager">A reference to the container manager in use.</param>
        /// <param name="gameOperationsApi">A reference to the game operations api.</param>
        /// <param name="combatOperationsApi">A reference to the combat operations api.</param>
        /// <param name="pathFinderAlgo">A reference to the path finding algorithm in use.</param>
        /// <param name="predefinedItemSet">A reference to the predefined item set declared.</param>
        /// <param name="scheduler">A reference to the scheduler instance.</param>
        public ElevatedOperationContext(
            ILogger logger,
            IMapDescriptor mapDescriptor,
            IMap map,
            ICreatureManager creatureManager,
            IItemFactory itemFactory,
            ICreatureFactory creatureFactory,
            IContainerManager containerManager,
            IGameOperationsApi gameOperationsApi,
            ICombatOperationsApi combatOperationsApi,
            IPathFinder pathFinderAlgo,
            IPredefinedItemSet predefinedItemSet,
            IScheduler scheduler)
            : base(
                logger,
                mapDescriptor,
                map,
                creatureManager,
                itemFactory,
                creatureFactory,
                containerManager,
                gameOperationsApi,
                combatOperationsApi,
                pathFinderAlgo,
                predefinedItemSet,
                scheduler)
        {
        }

        /// <summary>
        /// Gets the reference to the creature manager in use.
        /// </summary>
        public ICreatureManager CreatureManager => this.CreatureFinder as ICreatureManager;
    }
}
