// -----------------------------------------------------------------
// <copyright file="OperationContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Scheduling;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents a context for operations.
    /// </summary>
    public class OperationContext : EventContext, IOperationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationContext"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="mapDescriptor">A reference to the map descriptor in use.</param>
        /// <param name="tileAccessor">A reference to the tile accessor in use.</param>
        /// <param name="connectionFinder">A reference to the connection finder in use.</param>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        /// <param name="pathFinder">A reference to the path finder helper in use.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="creatureFactory">A reference to the creature factory in use.</param>
        /// <param name="operationFactory">A reference to the operation factory in use.</param>
        /// <param name="containerManager">A reference to the container manager in use.</param>
        /// <param name="scheduler">A reference to the scheduler instance.</param>
        /// <param name="combatApi">A refrence to the combat API.</param>
        /// <param name="gameApi">A reference to the game API.</param>
        /// <param name="eventRulesApi">A reference to the event rules API.</param>
        public OperationContext(
            ILogger logger,
            IMapDescriptor mapDescriptor,
            ITileAccessor tileAccessor,
            IConnectionFinder connectionFinder,
            ICreatureFinder creatureFinder,
            IPathFinder pathFinder,
            IItemFactory itemFactory,
            ICreatureFactory creatureFactory,
            IOperationFactory operationFactory,
            IContainerManager containerManager,
            IScheduler scheduler,
            ICombatApi combatApi,
            IGameApi gameApi,
            IEventRulesApi eventRulesApi)
            : base(logger)
        {
            mapDescriptor.ThrowIfNull(nameof(mapDescriptor));
            tileAccessor.ThrowIfNull(nameof(tileAccessor));
            connectionFinder.ThrowIfNull(nameof(connectionFinder));
            creatureFinder.ThrowIfNull(nameof(creatureFinder));
            pathFinder.ThrowIfNull(nameof(pathFinder));
            itemFactory.ThrowIfNull(nameof(itemFactory));
            creatureFactory.ThrowIfNull(nameof(creatureFactory));
            operationFactory.ThrowIfNull(nameof(operationFactory));
            containerManager.ThrowIfNull(nameof(containerManager));
            scheduler.ThrowIfNull(nameof(scheduler));
            combatApi.ThrowIfNull(nameof(combatApi));
            gameApi.ThrowIfNull(nameof(gameApi));
            eventRulesApi.ThrowIfNull(nameof(eventRulesApi));

            this.MapDescriptor = mapDescriptor;
            this.TileAccessor = tileAccessor;
            this.ConnectionFinder = connectionFinder;
            this.CreatureFinder = creatureFinder;
            this.PathFinder = pathFinder;
            this.ItemFactory = itemFactory;
            this.CreatureFactory = creatureFactory;
            this.OperationFactory = operationFactory;
            this.ContainerManager = containerManager;
            this.Scheduler = scheduler;
            this.CombatApi = combatApi;
            this.GameApi = gameApi;
            this.EventRulesApi = eventRulesApi;
        }

        /// <summary>
        /// Gets a reference to the map descriptor in use.
        /// </summary>
        public IMapDescriptor MapDescriptor { get; }

        /// <summary>
        /// Gets the reference to the tile accessor in use.
        /// </summary>
        public ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Gets the reference to the connection finder in use.
        /// </summary>
        public IConnectionFinder ConnectionFinder { get; }

        /// <summary>
        /// Gets the reference to the creature finder in use.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the reference to the path finder helper in use.
        /// </summary>
        public IPathFinder PathFinder { get; }

        /// <summary>
        /// Gets a reference to the item factory in use.
        /// </summary>
        public IItemFactory ItemFactory { get; }

        /// <summary>
        /// Gets a reference to the creature factory in use.
        /// </summary>
        public ICreatureFactory CreatureFactory { get; }

        /// <summary>
        /// Gets the reference to the operation factory.
        /// </summary>
        public IOperationFactory OperationFactory { get; }

        /// <summary>
        /// Gets a reference to the container manager in use.
        /// </summary>
        public IContainerManager ContainerManager { get; }

        /// <summary>
        /// Gets a reference to the scheduler in use.
        /// </summary>
        public IScheduler Scheduler { get; }

        /// <summary>
        /// Gets the reference to the combat api.
        /// </summary>
        public ICombatApi CombatApi { get; }

        /// <summary>
        /// Gets the reference to the game api.
        /// </summary>
        public IGameApi GameApi { get; }

        /// <summary>
        /// Gets the reference to the event rules api.
        /// </summary>
        public IEventRulesApi EventRulesApi { get; }
    }
}
