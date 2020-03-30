// -----------------------------------------------------------------
// <copyright file="ChangeItemOperation.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;

    /// <summary>
    /// Class that represents an event for an item change.
    /// </summary>
    public class ChangeItemOperation : BaseActionOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeItemOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the change.</param>
        /// <param name="typeId">The type id of the item being changed.</param>
        /// <param name="fromLocation">The location from which the item is being changed.</param>
        /// <param name="toTypeId">The type id of the item to change to.</param>
        /// <param name="carrierCreature">The creature who is carrying the thing, if any.</param>
        public ChangeItemOperation(
            uint requestorId,
            ushort typeId,
            Location fromLocation,
            ushort toTypeId,
            ICreature carrierCreature = null)
            : base(requestorId)
        {
            this.FromLocation = fromLocation;
            this.FromTypeId = typeId;
            this.FromCreature = carrierCreature;
            this.ToTypeId = toTypeId;
        }

        /// <summary>
        /// Gets the location from which the item is being changed.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Gets the type id from which the item is being changed.
        /// </summary>
        public ushort FromTypeId { get; }

        /// <summary>
        /// Gets the creature from which the item is being changed, if any.
        /// </summary>
        public ICreature FromCreature { get; }

        /// <summary>
        /// Gets the type id of the item to change to.
        /// </summary>
        public ushort ToTypeId { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            const byte FallbackIndex = 0xFF;

            var fromCylinder = this.FromLocation.DecodeCyclinder(context.TileAccessor, context.ContainerManager, out byte index, this.FromCreature);

            // Adjust index if this a map location.
            var item = (this.FromLocation.Type == LocationType.Map && (fromCylinder is ITile fromTile)) ? fromTile.FindItemWithId(this.FromTypeId) : fromCylinder?.FindItemAt(index);

            if (item == null || fromCylinder == null)
            {
                return;
            }

            IThing newItem = context.ItemFactory.Create(this.ToTypeId);

            if (newItem == null)
            {
                return;
            }

            // At this point, we have an item to change, and we were able to generate the new one, let's proceed.
            (bool replaceSuccessful, IThing replaceRemainder) = fromCylinder.ReplaceContent(context.ItemFactory, item, newItem, index, item.Amount);

            if (!replaceSuccessful || replaceRemainder != null)
            {
                this.AddContentToCylinderOrFallback(context, fromCylinder, FallbackIndex, ref replaceRemainder, includeTileAsFallback: true, this.GetRequestor(context.CreatureFinder));
            }

            if (replaceSuccessful)
            {
                if (fromCylinder is ITile atTile)
                {
                    context.Scheduler.ScheduleEvent(
                        new TileUpdatedNotification(
                            context.CreatureFinder,
                            () => context.ConnectionFinder.PlayersThatCanSee(context.CreatureFinder, atTile.Location),
                            new TileUpdatedNotificationArguments(atTile.Location, context.MapDescriptor.DescribeTile)));

                    // Evaluate if the new item triggers a collision.
                    context.EventRulesApi.EvaluateRules(this, EventRuleType.Collision, new CollisionEventRuleArguments(fromCylinder.Location, item, this.GetRequestor(context.CreatureFinder)));
                }
            }
        }
    }
}
