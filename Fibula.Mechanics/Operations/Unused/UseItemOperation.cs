// -----------------------------------------------------------------
// <copyright file="UseItemOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Actions
{
    using System;

    /// <summary>
    /// Class that represents an event for an item use.
    /// </summary>
    public class UseItemOperation : BaseActionOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="typeId">The id of the item being used.</param>
        /// <param name="fromLocation">The location from which the item is being used.</param>
        /// <param name="fromIndex">The index within the location from which the item is being used.</param>
        public UseItemOperation(uint requestorId, ushort typeId, Location fromLocation, byte fromIndex)
            : base(requestorId)
        {
            this.TypeId = typeId;
            this.FromLocation = fromLocation;
            this.FromIndex = fromIndex;
        }

        /// <summary>
        /// Gets the type id of the item being used.
        /// </summary>
        public ushort TypeId { get; }

        /// <summary>
        /// Gets the location from which the item is being used.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Gets the index within the location from which the item is being used.
        /// </summary>
        public byte FromIndex { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            var requestor = this.GetRequestor(context.CreatureFinder);

            var fromCylinder = this.FromLocation.DecodeCyclinder(context.TileAccessor, context.ContainerManager, out byte index, requestor);

            // Adjust index if this a map location.
            var item = (this.FromLocation.Type == LocationType.Map && (fromCylinder is ITile fromTile)) ? fromTile.FindItemAt(this.FromIndex) : fromCylinder?.FindItemAt(index);

            // Declare some pre-conditions.
            var itemFound = item != null;
            var isIntendedItem = item?.Type.ClientId == this.TypeId;

            if (!itemFound || !isIntendedItem)
            {
                // Silent fail.
                return;
            }

            var actionCooldownRemaining = requestor?.CalculateRemainingCooldownTime(this.ExhaustionType, context.Scheduler.CurrentTime) ?? TimeSpan.Zero;

            // Evaluate if this is a scripted use.
            if (context.EventRulesApi.EvaluateRules(this, EventRuleType.Use, new UseEventRuleArguments(item, requestor)))
            {
                return;
            }

            if (item.ChangesOnUse)
            {
                var changeItemOperation = context.OperationFactory.Create(
                    OperationType.ChangeItem,
                    new ChangeItemOperationCreationArguments(this.RequestorId, item.Type.TypeId, this.FromLocation, item.ChangeOnUseTo, item.Carrier ?? this.GetRequestor(context.CreatureFinder)));

                context.Scheduler.ScheduleEvent(changeItemOperation, actionCooldownRemaining);

                return;
            }

            if (requestor is IPlayer player)
            {
                if (item is IContainerItem container)
                {
                    var currentlyOpenAtPosition = context.ContainerManager.FindForCreature(player.Id, container);

                    if (currentlyOpenAtPosition == IContainerManager.UnsetContainerPosition)
                    {
                        // Player doesn't have this container open, so open.
                        var openContainerOperation = context.OperationFactory.Create(
                            OperationType.ContainerOpen,
                            new OpenContainerOperationCreationArguments(
                                this.RequestorId,
                                player,
                                container,
                                this.FromLocation.Type == LocationType.InsideContainer ? this.FromLocation.ContainerId : IContainerManager.UnsetContainerPosition));

                        context.Scheduler.ScheduleEvent(openContainerOperation, actionCooldownRemaining);
                    }
                    else
                    {
                        // Close the container for this player.
                        var closeContainerOperation = context.OperationFactory.Create(
                            OperationType.ContainerClose,
                            new CloseContainerOperationCreationArguments(this.RequestorId, player, container, currentlyOpenAtPosition));

                        context.Scheduler.ScheduleEvent(closeContainerOperation, actionCooldownRemaining);
                    }

                    return;
                }

                // Nothing was successful, send an error message.
                context.Scheduler.ScheduleEvent(
                    new TextMessageNotification(
                        () => context.ConnectionFinder.FindByPlayerId(player.Id).YieldSingleItem(),
                        new TextMessageNotificationArguments(MessageType.StatusSmall, OperationMessage.NotPossible)));
            }
        }
    }
}
