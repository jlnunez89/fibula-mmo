// -----------------------------------------------------------------
// <copyright file="NotificationContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Notifications
{
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Fibula.Server.Notifications.Contracts.Abstractions;
    using Serilog;

    public class NotificationContext : INotificationContext
    {
        public NotificationContext(
            ILogger logger,
            IMapDescriptor mapDescriptor,
            ICreatureFinder creatureFinder,
            IScheduler scheduler)
        {
            logger.ThrowIfNull(nameof(logger));
            mapDescriptor.ThrowIfNull(nameof(mapDescriptor));
            creatureFinder.ThrowIfNull(nameof(creatureFinder));
            scheduler.ThrowIfNull(nameof(scheduler));

            this.Logger = logger.ForContext<NotificationContext>();
            this.MapDescriptor = mapDescriptor;
            this.CreatureFinder = creatureFinder;
            this.Scheduler = scheduler;
        }

        public IMapDescriptor MapDescriptor { get; set; }

        public ICreatureFinder CreatureFinder { get; set; }

        public IScheduler Scheduler { get; set; }

        public ILogger Logger { get; set; }
    }
}
