// -----------------------------------------------------------------
// <copyright file="BaseOperation.cs" company="2Dudes">
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
    using System;
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Scheduling;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Delegates;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using OpenTibia.Server.Operations.Movements;
    using Serilog;

    /// <summary>
    /// Class that represents a common base between game operations.
    /// </summary>
    public abstract class BaseOperation : BaseEvent, IOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="operationContext">A reference to this operation's context.</param>
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        protected BaseOperation(ILogger logger, IOperationContext operationContext, uint requestorId)
            : base(logger, requestorId)
        {
            operationContext.ThrowIfNull(nameof(operationContext));

            this.Context = operationContext;
        }

        /// <summary>
        /// Event delegate that is called when a rule event evaluation is requested.
        /// </summary>
        public event EventRulesEvaluationTriggered EventRulesEvaluationTriggered;

        /// <summary>
        /// Gets a reference to this operation's context.
        /// </summary>
        public IOperationContext Context { get; }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public abstract ExhaustionType ExhaustionType { get; }

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public abstract TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Attempts to add content to the first possible cylinder that accepts it, on a chain of cylinders.
        /// </summary>
        /// <param name="cylinderChain">The chain of cylinders.</param>
        /// <param name="firstAttemptIndex">The index at which to attempt to add, only for the first attempted cylinder.</param>
        /// <param name="remainder">The remainder content to add, which overflows to the next cylinder in the chain.</param>
        /// <param name="requestorCreature">Optional. The creature requesting the addition of content.</param>
        /// <returns>True if the content was successfully added, false otherwise.</returns>
        protected bool AddContentToCylinderChain(IEnumerable<ICylinder> cylinderChain, byte firstAttemptIndex, ref IThing remainder, ICreature requestorCreature = null)
        {
            cylinderChain.ThrowIfNull(nameof(cylinderChain));

            const byte FallbackIndex = 0xFF;

            bool success = false;
            bool firstAttempt = true;

            foreach (var targetCylinder in cylinderChain)
            {
                IThing lastAddedThing = remainder;

                if (!success)
                {
                    (success, remainder) = targetCylinder.AddContent(this.Context.ItemFactory, remainder, firstAttempt ? firstAttemptIndex : FallbackIndex);
                }
                else if (remainder != null)
                {
                    (success, remainder) = targetCylinder.AddContent(this.Context.ItemFactory, remainder);
                }

                firstAttempt = false;

                if (success)
                {
                    if (targetCylinder is ITile targetTile)
                    {
                        this.Context.Scheduler.ScheduleEvent(
                                    new TileUpdatedNotification(
                                this.Logger,
                                this.Context.CreatureFinder,
                                () => this.Context.ConnectionFinder.PlayersThatCanSee(this.Context.CreatureFinder, targetTile.Location),
                                new TileUpdatedNotificationArguments(targetTile.Location, this.Context.MapDescriptor.DescribeTile)));

                        this.TriggerCollisionEventRules(new CollisionEventRuleArguments(targetCylinder.Location, lastAddedThing, requestorCreature));
                    }

                    this.TriggerMovementEventRules(new MovementEventRuleArguments(lastAddedThing, requestorCreature));
                }

                if (success && remainder == null)
                {
                    break;
                }
            }

            return success;
        }

        /// <summary>
        /// Triggers the <see cref="EventRulesEvaluationTriggered"/> event on this operation, for separation rules.
        /// </summary>
        /// <param name="eventRuleArguments">The arguments for the event rule evaluation.</param>
        /// <returns>True if the event is hooked up and the evaluation results are true, false otherwise.</returns>
        protected bool TriggerSeparationEventRules(IEventRuleArguments eventRuleArguments)
        {
            return this.EventRulesEvaluationTriggered?.Invoke(this, EventRuleType.Separation, eventRuleArguments) ?? false;
        }

        /// <summary>
        /// Triggers the <see cref="EventRulesEvaluationTriggered"/> event on this operation, for collision rules.
        /// </summary>
        /// <param name="eventRuleArguments">The arguments for the event rule evaluation.</param>
        /// <returns>True if the event is hooked up and the evaluation results are true, false otherwise.</returns>
        protected bool TriggerCollisionEventRules(IEventRuleArguments eventRuleArguments)
        {
            return this.EventRulesEvaluationTriggered?.Invoke(this, EventRuleType.Collision, eventRuleArguments) ?? false;
        }

        /// <summary>
        /// Triggers the <see cref="EventRulesEvaluationTriggered"/> event on this operation, for movement rules.
        /// </summary>
        /// <param name="eventRuleArguments">The arguments for the event rule evaluation.</param>
        /// <returns>True if the event is hooked up and the evaluation results are true, false otherwise.</returns>
        protected bool TriggerMovementEventRules(IEventRuleArguments eventRuleArguments)
        {
            return this.EventRulesEvaluationTriggered?.Invoke(this, EventRuleType.Movement, eventRuleArguments) ?? false;
        }

        /// <summary>
        /// Triggers the <see cref="EventRulesEvaluationTriggered"/> event on this operation, for use rules.
        /// </summary>
        /// <param name="eventRuleArguments">The arguments for the event rule evaluation.</param>
        /// <returns>True if the event is hooked up and the evaluation results are true, false otherwise.</returns>
        protected bool TriggerUseEventRules(IEventRuleArguments eventRuleArguments)
        {
            return this.EventRulesEvaluationTriggered?.Invoke(this, EventRuleType.Use, eventRuleArguments) ?? false;
        }

        /// <summary>
        /// Triggers the <see cref="EventRulesEvaluationTriggered"/> event on this operation, for multi-use rules.
        /// </summary>
        /// <param name="eventRuleArguments">The arguments for the event rule evaluation.</param>
        /// <returns>True if the event is hooked up and the evaluation results are true, false otherwise.</returns>
        protected bool TriggerMultiUseEventRules(IEventRuleArguments eventRuleArguments)
        {
            return this.EventRulesEvaluationTriggered?.Invoke(this, EventRuleType.MultiUse, eventRuleArguments) ?? false;
        }

        /// <summary>
        /// Schedules autowalking by a creature in the directions supplied.
        /// </summary>
        /// <param name="creature">The creature walking.</param>
        /// <param name="directions">The directions to follow.</param>
        /// <param name="stepIndex">Optional. The index of the current direction.</param>
        protected void AutoWalk(ICreature creature, Direction[] directions, int stepIndex = 0)
        {
            if (creature == null || directions.Length == 0 || stepIndex >= directions.Length)
            {
                return;
            }

            // A new request overrides and cancels any movement waiting to be retried.
            this.Context.Scheduler.CancelAllFor(creature.Id, typeof(IMovementOperation));

            var nextLocation = creature.Location.LocationAt(directions[stepIndex]);

            TimeSpan movementDelay = creature.CalculateRemainingCooldownTime(ExhaustionType.Movement, this.Context.Scheduler.CurrentTime);

            this.Context.Scheduler.ScheduleEvent(
                new MapToMapMovementOperation(
                    this.Logger,
                    this.Context,
                    creature.Id,
                    creature,
                    creature.Location,
                    nextLocation),
                movementDelay);

            if (directions.Length > 1)
            {
                // Add this request as the retry action, so that the request gets repeated when the player hits this location.
                creature.EnqueueRetryActionAtLocation(nextLocation, () => this.AutoWalk(creature, directions, stepIndex + 1));
            }
        }

        /// <summary>
        /// Sends a <see cref="TextMessageNotification"/> to the requestor of the operation, if there is one and it is a player.
        /// </summary>
        /// <param name="message">Optional. The message to send. Defaults to <see cref="OperationMessage.NotPossible"/>.</param>
        protected void SendFailureNotification(string message = OperationMessage.NotPossible)
        {
            if (this.RequestorId == 0 || !(this.Context.ConnectionFinder.FindByPlayerId(this.RequestorId) is IConnection connection) || connection == null)
            {
                return;
            }

            message.ThrowIfNullOrWhiteSpace();

            this.Context.Scheduler.ScheduleEvent(
                new TextMessageNotification(
                    this.Logger,
                    () => connection.YieldSingleItem(),
                    new TextMessageNotificationArguments(MessageType.StatusSmall, message)));
        }
    }
}
