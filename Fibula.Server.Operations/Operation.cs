// -----------------------------------------------------------------
// <copyright file="Operation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations
{
    using System;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Notifications.Arguments;
    using Fibula.Scheduling;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Fibula.Server.Contracts.Abstractions;
    using Fibula.Server.Contracts.Enumerations;
    using Fibula.Server.Contracts.Extensions;
    using Fibula.Server.Mechanics.Contracts.Extensions;
    using Fibula.Server.Notifications;
    using Fibula.Server.Operations.Contracts.Abstractions;

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
        }

        ///// <summary>
        ///// Gets the type of exhaustion that this operation produces.
        ///// </summary>
        //public abstract ExhaustionType ExhaustionType { get; }

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

            this.Execute(context as IOperationContext);
        }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">The execution context for this operation.</param>
        protected abstract void Execute(IOperationContext context);

        /// <summary>
        /// Attempts to add content to the first possible cylinder that accepts it, on a chain of cylinders.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        /// <param name="thingContainer">The first thing container to add to.</param>
        /// <param name="addIndex">The index at which to attempt to add, only for the first attempted cylinder.</param>
        /// <param name="remainder">The remainder content to add, which overflows to the next cylinder in the chain.</param>
        /// <param name="includeTileAsFallback">Optional. A value for whether to include tiles in the fallback chain.</param>
        /// <param name="requestorCreature">Optional. The creature requesting the addition of content.</param>
        /// <returns>True if the content was successfully added, false otherwise.</returns>
        protected bool AddContentToCylinderOrFallback(IOperationContext context, IThingContainer thingContainer, byte addIndex, ref IThing remainder, bool includeTileAsFallback = true, ICreature requestorCreature = null)
        {
            context.ThrowIfNull(nameof(context));
            thingContainer.ThrowIfNull(nameof(thingContainer));

            const byte FallbackIndex = 0xFF;

            bool success = false;
            bool firstAttempt = true;

            foreach (var targetCylinder in thingContainer.GetParentContainerHierarchy(includeTileAsFallback))
            {
                IThing lastAddedThing = remainder;

                if (!success)
                {
                    (success, remainder) = targetCylinder.AddContent(context.ItemFactory, remainder, firstAttempt ? addIndex : FallbackIndex);
                }
                else if (remainder != null)
                {
                    (success, remainder) = targetCylinder.AddContent(context.ItemFactory, remainder);
                }

                firstAttempt = false;

                if (success)
                {
                    if (targetCylinder is ITile targetTile)
                    {
                        new TileUpdatedNotification(
                            () => context.CreatureFinder.PlayersThatCanSee(context.TileAccessor, targetTile.Location),
                            new TileUpdatedNotificationArguments(targetTile.Location, context.MapDescriptor.DescribeTile))
                        .Execute(context);

                        //context.EventRulesApi.EvaluateRules(this, EventRuleType.Collision, new CollisionEventRuleArguments(targetCylinder.Location, lastAddedThing, requestorCreature));
                    }

                    //context.EventRulesApi.EvaluateRules(this, EventRuleType.Movement, new MovementEventRuleArguments(lastAddedThing, requestorCreature));
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

            context.Scheduler.ScheduleEvent(
                new TextMessageNotification(
                    () => player.YieldSingleItem(),
                    new TextMessageNotificationArguments(MessageType.StatusSmall, message)));
        }
    }
}
