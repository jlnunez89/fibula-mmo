// -----------------------------------------------------------------
// <copyright file="DeleteItemOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Environment
{
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using OpenTibia.Server.Operations.Actions;

    /// <summary>
    /// Class that represents an event for an item deletion.
    /// </summary>
    public class DeleteItemOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteItemOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the deletion.</param>
        /// <param name="typeId">The type id of the item being deleted.</param>
        /// <param name="atLocation">The location from which the item is being deleted.</param>
        public DeleteItemOperation(
            uint requestorId,
            ushort typeId,
            Location atLocation)
            : base(requestorId)
        {
            this.AtLocation = atLocation;
            this.TypeId = typeId;
        }

        /// <summary>
        /// Gets the location at which to delete the item.
        /// </summary>
        public Location AtLocation { get; }

        /// <summary>
        /// Gets the type id of the item to be deleted.
        /// </summary>
        public ushort TypeId { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            var requestor = this.GetRequestor(context.CreatureFinder);
            var atCylinder = this.AtLocation.DecodeCyclinder(context.TileAccessor, context.ContainerManager, out byte index, requestor);

            // Adjust index if this a map location.
            var item = (this.AtLocation.Type == LocationType.Map && (atCylinder is ITile fromTile)) ? fromTile.FindItemWithId(this.TypeId) : atCylinder?.FindItemAt(index);

            if (atCylinder == null || !(item is IThing itemAsThing))
            {
                return;
            }

            // At this point, we have an item to remove, let's proceed.
            (bool removeSuccessful, IThing remainder) = atCylinder.RemoveContent(context.ItemFactory, ref itemAsThing, index, amount: item.Amount);

            if (!removeSuccessful)
            {
                // Failing to remove the item from the original cylinder stops the entire operation.
                return;
            }

            if (atCylinder is ITile atTile)
            {
                //context.Scheduler.ScheduleEvent(
                //    new TileUpdatedNotification(
                //        context.CreatureFinder,
                //        () => context.ConnectionFinder.PlayersThatCanSee(context.CreatureFinder, atTile.Location),
                //        new TileUpdatedNotificationArguments(
                //            atTile.Location,
                //            context.MapDescriptor.DescribeTile)));

                new TileUpdatedNotification(
                    context.CreatureFinder,
                    () => context.ConnectionFinder.PlayersThatCanSee(context.CreatureFinder, atTile.Location),
                    new TileUpdatedNotificationArguments(
                        atTile.Location,
                        context.MapDescriptor.DescribeTile))
                .Execute(context);
            }

            context.EventRulesApi.EvaluateRules(this, EventRuleType.Separation, new SeparationEventRuleArguments(atCylinder.Location, item, this.GetRequestor(context.CreatureFinder)));
        }
    }
}
