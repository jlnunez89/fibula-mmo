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
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using OpenTibia.Server.Operations.Actions;
    using Serilog;

    /// <summary>
    /// Class that represents an event for an item deletion.
    /// </summary>
    public class DeleteItemOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Caches the requestor creature, if defined.
        /// </summary>
        private ICreature requestor = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteItemOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The operation's context.</param>
        /// <param name="requestorId">The id of the creature requesting the deletion.</param>
        /// <param name="typeId">The type id of the item being deleted.</param>
        /// <param name="atLocation">The location from which the item is being deleted.</param>
        public DeleteItemOperation(
            ILogger logger,
            IElevatedOperationContext context,
            uint requestorId,
            ushort typeId,
            Location atLocation)
            : base(logger, context, requestorId)
        {
            byte index = 0, subIndex = 0xFF;

            this.FromCylinder = atLocation.GetCyclinder(this.Context.TileAccessor, this.Context.ContainerManager, ref index, ref subIndex, this.Requestor);
            this.Item = atLocation.FindItemById(this.Context.TileAccessor, this.Context.ContainerManager, typeId, this.Requestor);
            this.FromIndex = subIndex;
        }

        /// <summary>
        /// Gets the creature that is requesting the event, if known.
        /// </summary>
        public ICreature Requestor
        {
            get
            {
                if (this.RequestorId > 0 && this.requestor == null)
                {
                    this.requestor = this.Context.CreatureFinder.FindCreatureById(this.RequestorId);
                }

                return this.requestor;
            }
        }

        /// <summary>
        /// Gets the item to delete.
        /// </summary>
        public IItem Item { get; }

        /// <summary>
        /// Gets the cylinder from which to delete the item.
        /// </summary>
        public ICylinder FromCylinder { get; }

        /// <summary>
        /// Gets the index at which to delete the item from the cyclinder.
        /// </summary>
        public byte FromIndex { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        public override void Execute()
        {
            if (this.FromCylinder == null || !(this.Item is IThing itemAsThing))
            {
                return;
            }

            // At this point, we have an item to remove, let's proceed.
            (bool removeSuccessful, IThing remainder) = this.FromCylinder.RemoveContent(this.Context.ItemFactory, ref itemAsThing, this.FromIndex, amount: this.Item.Amount);

            if (!removeSuccessful)
            {
                // Failing to remove the item from the original cylinder stops the entire operation.
                return;
            }

            if (this.FromCylinder is ITile fromTile)
            {
                this.Context.Scheduler.ScheduleEvent(
                    new TileUpdatedNotification(
                            this.Logger,
                            this.Context.CreatureFinder,
                            () => this.Context.ConnectionFinder.PlayersThatCanSee(this.Context.CreatureFinder, fromTile.Location),
                            new TileUpdatedNotificationArguments(
                                fromTile.Location,
                                this.Context.MapDescriptor.DescribeTile)));
            }

            // TODO: do we really need a requestor here?
            this.TriggerSeparationEventRules(new SeparationEventRuleArguments(this.FromCylinder.Location, this.Item, this.Requestor));
        }
    }
}
