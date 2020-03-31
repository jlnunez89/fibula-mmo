// -----------------------------------------------------------------
// <copyright file="EventRuleContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Events
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;

    public class EventRuleContext : IEventRuleContext
    {
        public EventRuleContext(IGameApi gameApi, ITileAccessor tileAccessor, IScheduler scheduler, IEventRuleArguments arguments)
        {
            gameApi.ThrowIfNull(nameof(gameApi));
            tileAccessor.ThrowIfNull(nameof(tileAccessor));
            scheduler.ThrowIfNull(nameof(scheduler));
            arguments.ThrowIfNull(nameof(arguments));

            this.GameApi = gameApi;
            this.TileAccessor = tileAccessor;
            this.Scheduler = scheduler;
            this.Arguments = arguments;
        }

        public IGameApi GameApi { get; }

        public ITileAccessor TileAccessor { get; }

        public IScheduler Scheduler { get; }

        public IEventRuleArguments Arguments { get; set; }
    }
}
