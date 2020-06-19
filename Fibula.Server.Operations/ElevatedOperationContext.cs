// -----------------------------------------------------------------
// <copyright file="ElevatedOperationContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations
{
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Fibula.Server.Operations.Contracts.Abstractions;
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
        /// <param name="tileAccessor">A reference to the tile accessor in use.</param>
        /// <param name="connectionManager">A reference to the connection manager in use.</param>
        /// <param name="creatureManager">A reference to the creature manager in use.</param>
        /// <param name="pathFinder">A reference to the path finder helper in use.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="creatureFactory">A reference to the creature factory in use.</param>
        /// <param name="operationFactory">A reference to the operation factory in use.</param>
        /// <param name="containerManager">A reference to the container manager in use.</param>
        /// <param name="scheduler">A reference to the scheduler instance.</param>
        /// <param name="combatApi">A reference to the combat API.</param>
        /// <param name="gameApi">A reference to the game API.</param>
        /// <param name="eventRulesApi">A reference to the event rules API.</param>
        public ElevatedOperationContext(
            ILogger logger,
            IMapDescriptor mapDescriptor,
            ITileAccessor tileAccessor,
            //IConnectionManager connectionManager,
            ICreatureManager creatureManager,
            //IPathFinder pathFinder,
            IItemFactory itemFactory,
            ICreatureFactory creatureFactory,
            IOperationFactory operationFactory,
            IContainerManager containerManager,
            IScheduler scheduler)
            //ICombatApi combatApi,
            //IGameApi gameApi,
            //IEventRulesApi eventRulesApi)
            : base(logger, mapDescriptor, tileAccessor, creatureManager, itemFactory, creatureFactory, operationFactory, containerManager, scheduler)
        {
        }

        /// <summary>
        /// Gets the reference to the connection manager in use.
        /// </summary>
        //public IConnectionManager ConnectionManager => this.ConnectionFinder as IConnectionManager;

        /// <summary>
        /// Gets the reference to the creature manager in use.
        /// </summary>
        public ICreatureManager CreatureManager => this.CreatureFinder as ICreatureManager;
    }
}
