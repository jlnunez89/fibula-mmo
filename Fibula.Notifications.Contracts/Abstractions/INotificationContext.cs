// -----------------------------------------------------------------
// <copyright file="INotificationContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Notifications.Contracts.Abstractions
{
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Interface for a notification context.
    /// </summary>
    public interface INotificationContext : IEventContext
    {
        /// <summary>
        /// Gets a reference to the map descriptor in use.
        /// </summary>
        IMapDescriptor MapDescriptor { get; }

        /// <summary>
        /// Gets the reference to the creature finder in use.
        /// </summary>
        ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets a reference to the scheduler in use.
        /// </summary>
        IScheduler Scheduler { get; }
    }
}