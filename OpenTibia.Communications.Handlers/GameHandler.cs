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

namespace OpenTibia.Communications.Handlers
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Operations.Arguments;
    using Serilog;

    /// <summary>
    /// Class that represents a handler that exposes a reference to the game instance, for convenience.
    /// </summary>
    public abstract class GameHandler : BaseHandler
    {
        /// <summary>
        /// The reference to the operation factory.
        /// </summary>
        private readonly IOperationFactory operationFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="operationFactory">A reference to the operation factory.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        protected GameHandler(ILogger logger, IOperationFactory operationFactory, IGameContext gameContext)
            : base(logger)
        {
            operationFactory.ThrowIfNull(nameof(operationFactory));
            gameContext.ThrowIfNull(nameof(gameContext));

            this.operationFactory = operationFactory;

            this.Context = gameContext;
        }

        /// <summary>
        /// Gets the reference to the game context.
        /// </summary>
        protected IGameContext Context { get; }

        /// <summary>
        /// Schedules autowalking by a creature in the directions supplied.
        /// </summary>
        /// <param name="creature">The creature walking.</param>
        /// <param name="directions">The directions to follow.</param>
        /// <param name="stepIndex">Optional. The index of the current direction.</param>
        protected void AutoWalk(ICreature creature, Direction[] directions, int stepIndex = 0)
        {
            if (directions.Length == 0 || stepIndex >= directions.Length)
            {
                return;
            }

            // A new request overrides and cancels any movement waiting to be retried.
            this.Context.Scheduler.CancelAllFor(creature.Id, typeof(IMovementOperation));

            var nextLocation = creature.Location.LocationAt(directions[stepIndex]);

            this.ScheduleNewOperation(
                    OperationType.MapToMapMovement,
                    new MapToMapMovementOperationCreationArguments(
                        creature.Id,
                        creature,
                        creature.Location,
                        nextLocation));

            if (directions.Length > 1)
            {
                // Add this request as the retry action, so that the request gets repeated when the player hits this location.
                creature.EnqueueRetryActionAtLocation(nextLocation, () => this.AutoWalk(creature, directions, stepIndex + 1));
            }
        }

        protected void ScheduleNewOperation(OperationType operationType, IOperationCreationArguments operationArguments, TimeSpan withDelay = default)
        {
            IOperation newOperation = this.operationFactory.Create(operationType, operationArguments);

            // Hook up the event rules event listener here.
            // This gets unhooked when the operation is processed.
            newOperation.EventRulesEvaluationTriggered += this.Context.Game.OnOperationEventRulesEvaluationTriggered;

            // Normalize delay to protect against negative time spans.
            var actualDelay = withDelay < TimeSpan.Zero ? TimeSpan.Zero : withDelay;

            // Add delay from current exhaustion of the requestor, if any.
            if (operationArguments.RequestorId > 0 && this.Context.CreatureFinder.FindCreatureById(operationArguments.RequestorId) is ICreature creature)
            {
                TimeSpan exhaustionDelay = creature.CalculateRemainingCooldownTime(newOperation.ExhaustionType, this.Context.Scheduler.CurrentTime);

                actualDelay += exhaustionDelay;
            }

            this.Context.Scheduler.ScheduleEvent(newOperation, this.Context.Scheduler.CurrentTime + actualDelay);
        }
    }
}