// -----------------------------------------------------------------
// <copyright file="GameContext.cs" company="2Dudes">
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
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a context for the game.
    /// </summary>
    public class GameContext : IGameContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameContext"/> class.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="tileAccessor"></param>
        /// <param name="creatureFinder"></param>
        /// <param name="pathFinder"></param>
        /// <param name="containerManager"></param>
        /// <param name="operationFactory"></param>
        /// <param name="scheduler"></param>
        public GameContext(
            IGame game,
            ITileAccessor tileAccessor,
            ICreatureFinder creatureFinder,
            IPathFinder pathFinder,
            IContainerManager containerManager,
            IOperationFactory operationFactory,
            IScheduler scheduler)
        {
            game.ThrowIfNull(nameof(game));
            tileAccessor.ThrowIfNull(nameof(tileAccessor));
            creatureFinder.ThrowIfNull(nameof(creatureFinder));
            pathFinder.ThrowIfNull(nameof(pathFinder));
            operationFactory.ThrowIfNull(nameof(operationFactory));
            scheduler.ThrowIfNull(nameof(scheduler));

            this.Game = game;
            this.TileAccessor = tileAccessor;
            this.CreatureFinder = creatureFinder;
            this.PathFinder = pathFinder;
            this.ContainerManager = containerManager;
            this.OperationFactory = operationFactory;
            this.Scheduler = scheduler;
        }

        /// <summary>
        /// Gets a reference to the game instance.
        /// </summary>
        public IGame Game { get; }

        /// <summary>
        /// Gets the reference to the tile accessor in use.
        /// </summary>
        public ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Gets the reference to the creature finder in use.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the reference to the path finder helper in use.
        /// </summary>
        public IPathFinder PathFinder { get; }

        /// <summary>
        /// Gets a reference to the container manager in use.
        /// </summary>
        public IContainerManager ContainerManager { get; }

        /// <summary>
        /// Gets a reference to the operation factory in use.
        /// </summary>
        public IOperationFactory OperationFactory { get; }

        /// <summary>
        /// Gets a reference to the scheduler in use.
        /// </summary>
        public IScheduler Scheduler { get; }
    }
}
