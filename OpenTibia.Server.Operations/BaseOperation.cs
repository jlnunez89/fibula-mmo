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
    using OpenTibia.Scheduling;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Delegates;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Operations.Notifications;
    using OpenTibia.Server.Operations.Notifications.Arguments;
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
        /// Gets the exhaustion cost time of this operation.
        /// </summary>
        public abstract TimeSpan ExhaustionCost { get; }

        /// <summary>
        /// Immediately attempts to perform an item change in behalf of the requesting creature, if any.
        /// </summary>
        /// <param name="item">The item being changed.</param>
        /// <param name="toTypeId">The type id of the item being changed to.</param>
        /// <param name="atCylinder">The cylinder at which the change is happening.</param>
        /// <param name="index">Optional. The index within the cylinder from which to change the item.</param>
        /// <param name="requestor">Optional. The creature requesting the change.</param>
        /// <returns>True if the item was successfully changed, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        protected bool PerformItemChange(IItem item, ushort toTypeId, ICylinder atCylinder, byte index = 0xFF, ICreature requestor = null)
        {
            const byte FallbackIndex = 0xFF;

            if (item == null || atCylinder == null)
            {
                return false;
            }

            IThing newItem = this.Context.ItemFactory.Create(toTypeId);

            if (newItem == null)
            {
                return false;
            }

            // At this point, we have an item to change, and we were able to generate the new one, let's proceed.
            (bool replaceSuccessful, IThing replaceRemainder) = atCylinder.ReplaceContent(this.Context.ItemFactory, item, newItem, index, item.Amount);

            if (!replaceSuccessful || replaceRemainder != null)
            {
                this.AddContentToCylinderChain(atCylinder.GetCylinderHierarchy(), FallbackIndex, ref replaceRemainder, requestor);
            }

            if (replaceSuccessful)
            {
                if (atCylinder is ITile atTile)
                {
                    this.Context.Scheduler.ImmediateEvent(
                            new TileUpdatedNotification(
                            this.Logger,
                            this.Context.CreatureFinder,
                            () => this.Context.ConnectionFinder.PlayersThatCanSee(this.Context.CreatureFinder, atTile.Location),
                            new TileUpdatedNotificationArguments(atTile.Location, this.Context.MapDescriptor.DescribeTile)));

                    this.TriggerCollisionEventRules(new CollisionEventRuleArguments(atCylinder.Location, item, requestor));
                }
            }

            return true;
        }

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
                        this.Context.Scheduler.ImmediateEvent(
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
    }
}
