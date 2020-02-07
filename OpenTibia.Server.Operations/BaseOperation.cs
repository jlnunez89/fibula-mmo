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
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Packets.Outgoing;
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
        /// Immediately attempts to perform an item movement between two cylinders.
        /// </summary>
        /// <param name="item">The item being moved.</param>
        /// <param name="fromCylinder">The cylinder from which the movement is being performed.</param>
        /// <param name="toCylinder">The cylinder to which the movement is being performed.</param>
        /// <param name="fromIndex">Optional. The index within the cylinder to move the item from.</param>
        /// <param name="toIndex">Optional. The index within the cylinder to move the item to.</param>
        /// <param name="amountToMove">Optional. The amount of the thing to move. Defaults to 1.</param>
        /// <param name="requestorCreature">Optional. The creature that this movement is being performed in behalf of, if any.</param>
        /// <returns>True if the movement was successfully performed, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        protected bool PerformItemMovement(IItem item, ICylinder fromCylinder, ICylinder toCylinder, byte fromIndex = 0xFF, byte toIndex = 0xFF, byte amountToMove = 1, ICreature requestorCreature = null)
        {
            const byte FallbackIndex = 0xFF;

            if (item == null || fromCylinder == null || toCylinder == null)
            {
                return false;
            }

            var sameCylinder = fromCylinder == toCylinder;

            if (sameCylinder && fromIndex == toIndex)
            {
                // no change at all.
                return true;
            }

            // Edge case, check if the moving item is the target container.
            if (item is IContainerItem containerItem && toCylinder is IContainerItem targetContainer && targetContainer.IsChildOf(containerItem))
            {
                return false;
            }

            IThing itemAsThing = item as IThing;

            (bool removeSuccessful, IThing removeRemainder) = fromCylinder.RemoveContent(this.Context.ItemFactory, ref itemAsThing, fromIndex, amount: amountToMove);

            if (!removeSuccessful)
            {
                // Failing to remove the item from the original cylinder stops the entire operation.
                return false;
            }

            if (fromCylinder is ITile fromTile)
            {
                this.Context.Scheduler.ImmediateEvent(
                    new TileUpdatedNotification(
                        this.Logger,
                        this.Context.CreatureFinder,
                        () => this.Context.ConnectionFinder.PlayersThatCanSee(this.Context.CreatureFinder, fromTile.Location),
                        new TileUpdatedNotificationArguments(fromTile.Location, this.Context.MapDescriptor.DescribeTile)));
            }

            this.TriggerSeparationEventRules(new SeparationEventRuleArguments(fromCylinder.Location, item, requestorCreature));

            IThing addRemainder = itemAsThing;

            if (sameCylinder && removeRemainder == null && fromIndex < toIndex)
            {
                // If the move happens within the same cylinder, we need to adjust the index of where we're adding, depending if it is before or after.
                toIndex--;
            }

            if (!this.AddContentToCylinderChain(toCylinder.GetCylinderHierarchy(includeTiles: false), toIndex, ref addRemainder, requestorCreature) || addRemainder != null)
            {
                // There is some rollback to do, as we failed to add the entire thing.
                IThing rollbackRemainder = addRemainder ?? item;

                if (!this.AddContentToCylinderChain(fromCylinder.GetCylinderHierarchy(), FallbackIndex, ref rollbackRemainder, requestorCreature))
                {
                    this.Logger.Error($"Rollback failed on {nameof(this.PerformItemMovement)}. Thing: {rollbackRemainder.DescribeForLogger()}");
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
        /// Performs a container open action for a player.
        /// </summary>
        /// <param name="forPlayer">The player for which the container is being opened.</param>
        /// <param name="containerItem">The container.</param>
        /// <param name="newId">The id as which to open the container.</param>
        protected void OpenContainer(IPlayer forPlayer, IContainerItem containerItem, byte newId = 0xFF)
        {
            forPlayer.ThrowIfNull(nameof(forPlayer));
            containerItem.ThrowIfNull(nameof(containerItem));

            if (containerItem.IsTracking(forPlayer.Id, out byte knownAsId) && newId == knownAsId)
            {
                // This player is has this container open already, with this id.
                return;
            }

            // Either this container is not opened by this player, or the Ids don't match.
            // In this case, we substitute whatever container is already open with this id (knownAsId) with the container, as the newId.
            var currentContainer = forPlayer.GetContainerById(newId);

            if (currentContainer != null)
            {
                forPlayer.CloseContainerWithId(newId);

                // Unsubscribe from the event listeners on the container that we're substituting, if any.
                // We only unsubscribe from the listeners if no one cares about it anymore.
                if (currentContainer.OpenedBy.Count == 0)
                {
                    currentContainer.ContentAdded -= this.OnContainerContentAdded;
                    currentContainer.ContentRemoved -= this.OnContainerContentRemoved;
                    currentContainer.ContentUpdated -= this.OnContainerContentUpdated;

                    currentContainer.ThingChanged -= this.OnContainerChanged;
                }
            }

            // Subscribe to the event listeners of this container if we're not doing it already.
            // We only subscribe to the listeners if there was no one insterested in it before.
            if (containerItem != null && containerItem.OpenedBy.Count == 0)
            {
                containerItem.ContentAdded += this.OnContainerContentAdded;
                containerItem.ContentRemoved += this.OnContainerContentRemoved;
                containerItem.ContentUpdated += this.OnContainerContentUpdated;

                containerItem.ThingChanged += this.OnContainerChanged;
            }

            // Now actually open the container for this player (updates the container's OpenedBy list).
            byte containerId = forPlayer.OpenContainerAt(containerItem, newId);

            this.Context.Scheduler.ImmediateEvent(
                new GenericNotification(
                    this.Logger,
                    () => this.Context.ConnectionFinder.FindByPlayerId(forPlayer.Id).YieldSingleItem(),
                    new GenericNotificationArguments(
                        new ContainerOpenPacket(
                            containerId,
                            containerItem.ThingId,
                            containerItem.Type.Name,
                            containerItem.Capacity,
                            (containerItem.ParentCylinder is IContainerItem parentContainer) && parentContainer.Type.TypeId != 0,
                            containerItem.Content))));
        }

        /// <summary>
        /// Performs a container close action for a player.
        /// </summary>
        /// <param name="forPlayer">The player for which the container is being opened.</param>
        /// <param name="containerItem">The container being closed.</param>
        /// <param name="asContainerId">The id of the container being closed, as seen by the player.</param>
        protected void CloseContainer(IPlayer forPlayer, IContainerItem containerItem, byte asContainerId)
        {
            forPlayer.ThrowIfNull(nameof(forPlayer));
            containerItem.ThrowIfNull(nameof(containerItem));

            forPlayer.CloseContainerWithId(asContainerId);

            // clean up events if no one else cares about this container.
            if (containerItem.OpenedBy.Count == 0)
            {
                containerItem.ContentAdded -= this.OnContainerContentAdded;
                containerItem.ContentRemoved -= this.OnContainerContentRemoved;
                containerItem.ContentUpdated -= this.OnContainerContentUpdated;

                containerItem.ThingChanged -= this.OnContainerChanged;
            }

            this.Context.Scheduler.ImmediateEvent(
                new GenericNotification(
                    this.Logger,
                    () => this.Context.ConnectionFinder.FindByPlayerId(forPlayer.Id).YieldSingleItem(),
                    new GenericNotificationArguments(new ContainerClosePacket(asContainerId))));
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
        /// Handles an event from a container content added.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="addedItem">The item that was added.</param>
        private void OnContainerContentAdded(IContainerItem container, IItem addedItem)
        {
            // The request has to be sent this way since the container id may be different for each player.
            foreach (var (creatureId, containerId) in container.OpenedBy.ToList())
            {
                if (!(this.Context.CreatureFinder.FindCreatureById(creatureId) is IPlayer player))
                {
                    continue;
                }

                this.Context.Scheduler.ImmediateEvent(
                    new GenericNotification(
                        this.Logger,
                        () => this.Context.ConnectionFinder.FindByPlayerId(player.Id).YieldSingleItem(),
                        new GenericNotificationArguments(new ContainerAddItemPacket(containerId, addedItem))));
            }
        }

        /// <summary>
        /// Handles an event from a container content removed.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="indexRemoved">The index that was removed.</param>
        private void OnContainerContentRemoved(IContainerItem container, byte indexRemoved)
        {
            // The request has to be sent this way since the container id may be different for each player.
            foreach (var (creatureId, containerId) in container.OpenedBy.ToList())
            {
                if (!(this.Context.CreatureFinder.FindCreatureById(creatureId) is IPlayer player))
                {
                    continue;
                }

                this.Context.Scheduler.ImmediateEvent(
                    new GenericNotification(
                        this.Logger,
                        () => this.Context.ConnectionFinder.FindByPlayerId(player.Id).YieldSingleItem(),
                        new GenericNotificationArguments(new ContainerRemoveItemPacket(indexRemoved, containerId))));
            }
        }

        /// <summary>
        /// Handles an event from a container content updated.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="indexOfUpdated">The index that was updated.</param>
        /// <param name="updatedItem">The updated item.</param>
        private void OnContainerContentUpdated(IContainerItem container, byte indexOfUpdated, IItem updatedItem)
        {
            if (updatedItem == null)
            {
                return;
            }

            // The request has to be sent this way since the container id may be different for each player.
            foreach (var (creatureId, containerId) in container.OpenedBy.ToList())
            {
                if (!(this.Context.CreatureFinder.FindCreatureById(creatureId) is IPlayer player))
                {
                    continue;
                }

                this.Context.Scheduler.ImmediateEvent(
                    new GenericNotification(
                        this.Logger,
                        () => this.Context.ConnectionFinder.FindByPlayerId(player.Id).YieldSingleItem(),
                        new GenericNotificationArguments(new ContainerUpdateItemPacket((byte)indexOfUpdated, containerId, updatedItem))));
            }
        }

        /// <summary>
        /// Handles a change event from a container.
        /// </summary>
        /// <param name="containerThatChangedAsThing">The container that changed.</param>
        /// <param name="eventArgs">The event arguments of the change.</param>
        private void OnContainerChanged(IThing containerThatChangedAsThing, ThingStateChangedEventArgs eventArgs)
        {
            if (!(containerThatChangedAsThing is IContainerItem containerItem) || !eventArgs.PropertyChanged.Equals(nameof(containerItem.Location)))
            {
                return;
            }

            if (containerItem.CarryLocation != null)
            {
                // Container is held by a creature, which is the only one that should have access now.
                var creatureHoldingTheContainer = containerItem.Carrier;

                if (creatureHoldingTheContainer != null)
                {
                    foreach (var (creatureId, containerId) in containerItem.OpenedBy.ToList())
                    {
                        if (creatureHoldingTheContainer.Id == creatureId || !(this.Context.CreatureFinder.FindCreatureById(creatureId) is IPlayer player))
                        {
                            continue;
                        }

                        this.CloseContainer(player, containerItem, containerId);
                    }
                }
            }
            else if (containerItem.Location.Type == LocationType.Map)
            {
                // Container was dropped or placed in a container that ultimately sits on the map, figure out which creatures are still in range.
                foreach (var (creatureId, containerId) in containerItem.OpenedBy.ToList())
                {
                    if (!(this.Context.CreatureFinder.FindCreatureById(creatureId) is IPlayer player))
                    {
                        continue;
                    }

                    var locationDiff = containerItem.Location - player.Location;

                    if (locationDiff.MaxValueIn2D > 1 || locationDiff.Z != 0)
                    {
                        this.CloseContainer(player, containerItem, containerId);
                    }
                }
            }
        }
    }
}
