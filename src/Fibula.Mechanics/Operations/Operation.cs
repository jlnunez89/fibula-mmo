// -----------------------------------------------------------------
// <copyright file="Operation.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using System;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Extensions;
    using Fibula.Mechanics.Notifications;
    using Fibula.Scheduling;
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a common base between game operations.
    /// </summary>
    public abstract class Operation : BaseEvent, IOperation
    {
        /// <summary>
        /// Caches the requestor creature, if defined.
        /// </summary>
        private ICreature requestor = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        protected Operation(uint requestorId)
            : base(requestorId)
        {
            this.CanBeCancelled = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be cancelled.
        /// </summary>
        public override bool CanBeCancelled { get; protected set; }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public abstract ExhaustionType ExhaustionType { get; }

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public abstract TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Gets the creature that is requesting the event, if known.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        /// <returns>The creature that requested the operation, or null if there wasn't any.</returns>
        public ICreature GetRequestor(ICreatureFinder creatureFinder)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            if (this.requestor == null && this.RequestorId > 0)
            {
                this.requestor = creatureFinder.FindCreatureById(this.RequestorId);
            }

            return this.requestor;
        }

        /// <summary>
        /// Executes the event logic.
        /// </summary>
        /// <param name="context">The execution context.</param>
        public override void Execute(IEventContext context)
        {
            context.ThrowIfNull(nameof(context));

            if (!typeof(IOperationContext).IsAssignableFrom(context.GetType()))
            {
                throw new ArgumentException($"{nameof(context)} must be an {nameof(IOperationContext)}.");
            }

            var operationContext = context as IOperationContext;

            // Reset the operation's Repeat property, to avoid implementations running perpetually.
            this.RepeatAfter = TimeSpan.MinValue;

            this.Execute(operationContext);

            // Add any exhaustion for the requestor of the operation, if any.
            if (this.GetRequestor(operationContext.CreatureFinder) is ICreatureWithExhaustion requestor)
            {
                requestor.AddExhaustion(this.ExhaustionType, operationContext.Scheduler.CurrentTime, this.ExhaustionCost);
            }
        }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">The execution context for this operation.</param>
        protected abstract void Execute(IOperationContext context);

        /// <summary>
        /// Attempts to add content to the first possible parent container that accepts it, on a chain of parent containers.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        /// <param name="thingContainer">The first thing container to add to.</param>
        /// <param name="remainder">The remainder content to add, which overflows to the next container in the chain.</param>
        /// <param name="addIndex">The index at which to attempt to add, only for the first attempted container.</param>
        /// <param name="includeTileAsFallback">Optional. A value for whether to include tiles in the fallback chain.</param>
        /// <param name="requestorCreature">Optional. The creature requesting the addition of content.</param>
        /// <returns>True if the content was successfully added, false otherwise.</returns>
        protected bool AddContentToContainerOrFallback(IOperationContext context, IThingContainer thingContainer, ref IThing remainder, byte addIndex = byte.MaxValue, bool includeTileAsFallback = true, ICreature requestorCreature = null)
        {
            context.ThrowIfNull(nameof(context));
            thingContainer.ThrowIfNull(nameof(thingContainer));

            const byte FallbackIndex = byte.MaxValue;

            bool success = false;
            bool firstAttempt = true;

            foreach (var targetContainer in thingContainer.GetParentContainerHierarchy(includeTileAsFallback))
            {
                IThing lastAddedThing = remainder;

                if (!success)
                {
                    (success, remainder) = targetContainer.AddContent(context.ItemFactory, remainder, firstAttempt ? addIndex : FallbackIndex);
                }
                else if (remainder != null)
                {
                    (success, remainder) = targetContainer.AddContent(context.ItemFactory, remainder);
                }

                firstAttempt = false;

                if (success)
                {
                    if (targetContainer is ITile targetTile)
                    {
                        this.SendNotification(
                            context,
                            new TileUpdatedNotification(
                                () => context.Map.PlayersThatCanSee(targetTile.Location),
                                targetTile.Location,
                                context.MapDescriptor.DescribeTile));

                        // context.EventRulesApi.EvaluateRules(this, EventRuleType.Collision, new CollisionEventRuleArguments(targetContainer.Location, lastAddedThing, requestorCreature));
                    }

                    // context.EventRulesApi.EvaluateRules(this, EventRuleType.Movement, new MovementEventRuleArguments(lastAddedThing, requestorCreature));
                }

                if (success && remainder == null)
                {
                    break;
                }
            }

            return success;
        }

        /// <summary>
        /// Sends a <see cref="TextMessageNotification"/> to the requestor of the operation, if there is one and it is a player.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        /// <param name="message">Optional. The message to send. Defaults to <see cref="OperationMessage.NotPossible"/>.</param>
        protected void DispatchTextNotification(IOperationContext context, string message = OperationMessage.NotPossible)
        {
            if (this.RequestorId == 0 || !(context.CreatureFinder.FindCreatureById(this.RequestorId) is IPlayer player))
            {
                return;
            }

            message.ThrowIfNullOrWhiteSpace();

            this.SendNotification(context, new TextMessageNotification(() => player.YieldSingleItem(), MessageType.StatusSmall, message));
        }

        /// <summary>
        /// Sends a notification synchronously.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        /// <param name="notification">The notification to send.</param>
        protected void SendNotification(IOperationContext context, INotification notification)
        {
            notification.ThrowIfNull(nameof(notification));

            notification.Send(new NotificationContext(context.Logger, context.MapDescriptor, context.CreatureFinder));
        }
    }
}
