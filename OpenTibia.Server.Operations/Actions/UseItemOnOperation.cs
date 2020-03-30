// -----------------------------------------------------------------
// <copyright file="UseItemOnOperation.cs" company="2Dudes">
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

    /// <summary>
    /// Class that represents an event for an item use.
    /// </summary>
    public class UseItemOnOperation : BaseActionOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemOnOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="typeId">The id of the item being used.</param>
        /// <param name="fromLocation">The location from which the item is being used.</param>
        public UseItemOnOperation(uint requestorId, ushort fromTypeId, Location fromLocation, byte fromIndex, ushort toThingId, Location toLocation, byte toIndex)
            : base(requestorId)
        {
            this.FromTypeId = fromTypeId;
            this.FromLocation = fromLocation;
            this.FromIndex = fromIndex;
            this.ToThingId = toThingId;
            this.ToLocation = toLocation;
            this.ToIndex = toIndex;
        }

        /// <summary>
        /// Gets the type id of the item being used.
        /// </summary>
        public ushort FromTypeId { get; }

        /// <summary>
        /// Gets the location from which the item is being used.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Gets the index within the location from which the item is being used.
        /// </summary>
        public byte FromIndex { get; }

        /// <summary>
        /// Gets the id of the thing on which the item is being used.
        /// </summary>
        public ushort ToThingId { get; }

        /// <summary>
        /// Gets the location of the thing on which the item is being used.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Gets the index within the location of the thing on which the item is being used.
        /// </summary>
        public byte ToIndex { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            var requestor = this.GetRequestor(context.CreatureFinder);

            var fromCylinder = this.FromLocation.DecodeCyclinder(context.TileAccessor, context.ContainerManager, out byte fromIndex, requestor);
            var toCylinder = this.ToLocation.DecodeCyclinder(context.TileAccessor, context.ContainerManager, out byte toIndex, requestor);

            // Adjust index if this a map location.
            var item = (this.FromLocation.Type == LocationType.Map && (fromCylinder is ITile fromTile)) ? fromTile.FindItemWithId(this.FromTypeId) : fromCylinder?.FindItemAt(fromIndex);
            var targetThing = (this.ToLocation.Type == LocationType.Map && (toCylinder is ITile toTile)) ? toTile.GetTopThingByOrder(context.CreatureFinder, this.ToIndex) : fromCylinder?.FindItemAt(fromIndex);

            if (item == null || fromCylinder == null || targetThing == null || targetThing.ThingId != this.ToThingId)
            {
                return;
            }

            var actionCooldownRemaining = requestor?.CalculateRemainingCooldownTime(this.ExhaustionType, context.Scheduler.CurrentTime) ?? TimeSpan.Zero;

            // Evaluate if this is a scripted use.
            if (context.EventRulesApi.EvaluateRules(this, EventRuleType.MultiUse, new MultiUseEventRuleArguments(item, targetThing, requestor)))
            {
                return;
            }

            if (requestor is IPlayer player)
            {
                // Nothing was successful, send an error message.
                context.Scheduler.ScheduleEvent(
                    new TextMessageNotification(
                        () => context.ConnectionFinder.FindByPlayerId(player.Id).YieldSingleItem(),
                        new TextMessageNotificationArguments(MessageType.StatusSmall, OperationMessage.NotPossible)));
            }
        }
    }
}
