// -----------------------------------------------------------------
// <copyright file="UseItemOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Actions
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using Serilog;

    /// <summary>
    /// Class that represents an event for an item use.
    /// </summary>
    public class UseItemOperation : BaseActionOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The context of the operation.</param>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="typeId">The id of the item being used.</param>
        /// <param name="fromLocation">The location from which the item is being used.</param>
        /// <param name="fromStackPos">The position in the stack from which the item is being used.</param>
        /// <param name="index">The index of the item being used.</param>
        public UseItemOperation(ILogger logger, IOperationContext context, uint requestorId, ushort typeId, Location fromLocation, byte fromStackPos = byte.MaxValue, byte index = 1)
            : base(logger, context, requestorId)
        {
            this.ItemToUse = fromLocation.FindItemById(this.Context.TileAccessor, this.Context.ContainerManager, typeId, this.Requestor);
            this.FromCylinder = fromLocation.GetCyclinder(this.Context.TileAccessor, this.Context.ContainerManager, ref fromStackPos, ref index, this.Requestor);
            this.Index = index;
            this.ContainerPosition = fromLocation.Type == LocationType.InsideContainer ? (byte?)fromLocation.ContainerId : null;
        }

        /// <summary>
        /// Gets the reference to the item being used.
        /// </summary>
        public IItem ItemToUse { get; }

        /// <summary>
        /// Gets the cylinder from which the item is being used.
        /// </summary>
        public ICylinder FromCylinder { get; }

        /// <summary>
        /// Gets the index withing the cylinder from which the item is being used.
        /// </summary>
        public byte Index { get; }

        /// <summary>
        /// Gets the container position of the item being used, if applicable.
        /// </summary>
        public byte? ContainerPosition { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        public override void Execute()
        {
            if (this.ItemToUse == null || this.FromCylinder == null)
            {
                return;
            }

            var actionCooldownRemaining = this.Requestor?.CalculateRemainingCooldownTime(this.ExhaustionType, this.Context.Scheduler.CurrentTime) ?? TimeSpan.Zero;

            if (this.TriggerUseEventRules(new UseEventRuleArguments(this.ItemToUse, this.Requestor)))
            {
                return;
            }

            if (this.ItemToUse.ChangesOnUse)
            {
                this.Context.Scheduler.ScheduleEvent(
                    new ChangeItemOperation(this.Logger, this.Context, this.RequestorId, this.ItemToUse, this.FromCylinder, this.Index, this.ItemToUse.ChangeOnUseTo),
                    actionCooldownRemaining);

                return;
            }

            if (this.Requestor is IPlayer player)
            {
                if (this.ItemToUse is IContainerItem container)
                {
                    var currentlyOpenAtPosition = this.Context.ContainerManager.FindForCreature(player.Id, container);

                    if (currentlyOpenAtPosition == IContainerManager.UnsetContainerPosition)
                    {
                        // Player doesn't have this container open, so open.
                        this.Context.Scheduler.ScheduleEvent(
                            new OpenContainerOperation(this.Logger, this.Context, player, container, this.ContainerPosition ?? IContainerManager.UnsetContainerPosition),
                            actionCooldownRemaining);
                    }
                    else
                    {
                        // Close the container for this player.
                        this.Context.Scheduler.ScheduleEvent(
                            new CloseContainerOperation(this.Logger, this.Context, player, container, currentlyOpenAtPosition),
                            actionCooldownRemaining);
                    }

                    return;
                }

                // Nothing was successful, send an error message.
                this.Context.Scheduler.ScheduleEvent(
                    new TextMessageNotification(
                        this.Logger,
                        () => this.Context.ConnectionFinder.FindByPlayerId(player.Id).YieldSingleItem(),
                        new TextMessageNotificationArguments(MessageType.StatusSmall, OperationMessage.NotPossible)));
            }
        }
    }
}
