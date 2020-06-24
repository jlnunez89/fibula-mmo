// -----------------------------------------------------------------
// <copyright file="IEventRuleContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Interface for an event rule execution context.
    /// </summary>
    public interface IEventRuleContext
    {
        IGameOperationsApi GameApi { get; }

        ITileAccessor TileAccessor { get; }

        IScheduler Scheduler { get; }

        IEventRuleArguments Arguments { get; set; }
    }
}