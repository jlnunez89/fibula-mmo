// -----------------------------------------------------------------
// <copyright file="ConditionContext.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Conditions
{
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Scheduling;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents a context for conditions.
    /// </summary>
    public class ConditionContext : EventContext, IConditionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionContext"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="mapDescriptor">A reference to the map descriptor in use.</param>
        /// <param name="map">A reference to the map in use.</param>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="scheduler">A reference to the scheduler instance.</param>
        public ConditionContext(
            ILogger logger,
            IMapDescriptor mapDescriptor,
            IMap map,
            ICreatureFinder creatureFinder,
            IItemFactory itemFactory,
            IScheduler scheduler)
            : base(logger, () => scheduler.CurrentTime)
        {
            mapDescriptor.ThrowIfNull(nameof(mapDescriptor));
            map.ThrowIfNull(nameof(map));
            creatureFinder.ThrowIfNull(nameof(creatureFinder));
            itemFactory.ThrowIfNull(nameof(itemFactory));
            scheduler.ThrowIfNull(nameof(scheduler));

            this.MapDescriptor = mapDescriptor;
            this.Map = map;
            this.CreatureFinder = creatureFinder;
            this.ItemFactory = itemFactory;
            this.Scheduler = scheduler;
        }

        /// <summary>
        /// Gets a reference to the map descriptor in use.
        /// </summary>
        public IMapDescriptor MapDescriptor { get; }

        /// <summary>
        /// Gets the reference to the map.
        /// </summary>
        public IMap Map { get; }

        /// <summary>
        /// Gets the reference to the creature finder in use.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets a reference to the item factory in use.
        /// </summary>
        public IItemFactory ItemFactory { get; }

        /// <summary>
        /// Gets a reference to the scheduler in use.
        /// </summary>
        public IScheduler Scheduler { get; }
    }
}
