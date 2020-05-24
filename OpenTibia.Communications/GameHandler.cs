// -----------------------------------------------------------------
// <copyright file="GameHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents a handler that exposes a reference to the game instance, for convenience.
    /// </summary>
    public abstract class GameHandler : BaseHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        protected GameHandler(ILogger logger, IGameContext gameContext)
            : base(logger)
        {
            gameContext.ThrowIfNull(nameof(gameContext));

            this.Context = gameContext;
        }

        /// <summary>
        /// Gets the reference to the game context.
        /// </summary>
        protected IGameContext Context { get; }

        protected void ScheduleNewOperation(IOperation newOperation, TimeSpan withDelay = default)
        {
            newOperation.ThrowIfNull(nameof(newOperation));

            // Normalize delay to protect against negative time spans.
            var actualDelay = withDelay < TimeSpan.Zero ? TimeSpan.Zero : withDelay;

            // Add delay from current exhaustion of the requestor, if any.
            if (newOperation.RequestorId > 0 && this.Context.CreatureFinder.FindCreatureById(newOperation.RequestorId) is ICreature creature)
            {
                TimeSpan exhaustionDelay = creature.CalculateRemainingCooldownTime(newOperation.ExhaustionType, this.Context.Scheduler.CurrentTime);

                actualDelay += exhaustionDelay;
            }

            this.Context.Scheduler.ScheduleEvent(newOperation, actualDelay);
        }
    }
}