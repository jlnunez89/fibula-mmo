// -----------------------------------------------------------------
// <copyright file="IOperationContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Contracts.Abstractions
{
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Fibula.Server.Map.Contracts.Abstractions;

    /// <summary>
    /// Interface for an operation context.
    /// </summary>
    public interface IOperationContext : IEventContext
    {
        /// <summary>
        /// Gets a reference to the map descriptor in use.
        /// </summary>
        IMapDescriptor MapDescriptor { get; }

        /// <summary>
        /// Gets the reference to the tile accessor in use.
        /// </summary>
        ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Gets the reference to the creature finder in use.
        /// </summary>
        ICreatureFinder CreatureFinder { get; }

        ///// <summary>
        ///// Gets the reference to the path finder helper in use.
        ///// </summary>
        //IPathFinder PathFinder { get; }

        /// <summary>
        /// Gets a reference to the item factory in use.
        /// </summary>
        IItemFactory ItemFactory { get; }

        /// <summary>
        /// Gets a reference to the creature factory in use.
        /// </summary>
        ICreatureFactory CreatureFactory { get; }

        /// <summary>
        /// Gets the reference to the operation factory.
        /// </summary>
        IOperationFactory OperationFactory { get; }

        ///// <summary>
        ///// Gets a reference to the container manager in use.
        ///// </summary>
        //IContainerManager ContainerManager { get; }

        /// <summary>
        /// Gets a reference to the scheduler in use.
        /// </summary>
        IScheduler Scheduler { get; }

        ///// <summary>
        ///// Gets the reference to the combat api.
        ///// </summary>
        //ICombatApi CombatApi { get; }

        ///// <summary>
        ///// Gets the reference to the game api.
        ///// </summary>
        //IGameOperationsApi GameApi { get; }

        ///// <summary>
        ///// Gets the reference to the event rules api.
        ///// </summary>
        //IEventRulesApi EventRulesApi { get; }
    }
}