// -----------------------------------------------------------------
// <copyright file="ElevatedOperationContext.cs" company="2Dudes">
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
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an elevated context for operations.
    /// </summary>
    public class ElevatedOperationContext : OperationContext, IElevatedOperationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatedOperationContext"/> class.
        /// </summary>
        /// <param name="mapDescriptor"></param>
        /// <param name="eventRuleService"></param>
        /// <param name="tileAccessor"></param>
        /// <param name="connectionManager"></param>
        /// <param name="creatureManager"></param>
        /// <param name="pathFinder"></param>
        /// <param name="itemFactory"></param>
        /// <param name="creatureFactory"></param>
        /// <param name="containerManager"></param>
        /// <param name="scheduler"></param>
        public ElevatedOperationContext(
            IMapDescriptor mapDescriptor,
            ITileAccessor tileAccessor,
            IConnectionManager connectionManager,
            ICreatureManager creatureManager,
            IPathFinder pathFinder,
            IItemFactory itemFactory,
            ICreatureFactory creatureFactory,
            IContainerManager containerManager,
            IScheduler scheduler)
            : base(mapDescriptor, tileAccessor, connectionManager, creatureManager, pathFinder, itemFactory, creatureFactory, containerManager, scheduler)
        {
        }

        /// <summary>
        /// Gets the reference to the connection manager in use.
        /// </summary>
        public IConnectionManager ConnectionManager => this.ConnectionFinder as IConnectionManager;

        /// <summary>
        /// Gets the reference to the creature manager in use.
        /// </summary>
        public ICreatureManager CreatureManager => this.CreatureFinder as ICreatureManager;
    }
}
