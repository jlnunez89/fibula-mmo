// -----------------------------------------------------------------
// <copyright file="IGameContext.cs" company="2Dudes">
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
    using OpenTibia.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Interface for a game context.
    /// </summary>
    public interface IGameContext
    {
        /// <summary>
        /// Gets a reference to the game instance.
        /// </summary>
        IGame Game { get; }

        /// <summary>
        /// Gets the reference to the tile accessor in use.
        /// </summary>
        ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Gets the reference to the creature finder in use.
        /// </summary>
        ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the reference to the path finder helper in use.
        /// </summary>
        IPathFinder PathFinder { get; }

        /// <summary>
        /// Gets a reference to the container manager in use.
        /// </summary>
        IContainerManager ContainerManager { get; }

        /// <summary>
        /// Gets the reference to the operation factory in use.
        /// </summary>
        IOperationFactory OperationFactory { get; }

        /// <summary>
        /// Gets the reference to the event rules API.
        /// </summary>
        IEventRulesApi EventRulesApi { get; }

        /// <summary>
        /// Gets a reference to the scheduler in use.
        /// </summary>
        IScheduler Scheduler { get; }
    }
}