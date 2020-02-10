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
    using OpenTibia.Server.Operations.Actions;
    using OpenTibia.Server.Operations.Notifications;
    using OpenTibia.Server.Operations.Notifications.Arguments;
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
            this.ActionsOnPass.Add(() =>
            {
                byte index = 0, subIndex = 0;

                var fromCylinder = atLocation.GetCyclinder(this.Context.TileAccessor, this.Context.ContainerManager, ref index, ref subIndex, this.Requestor);
                var item = atLocation.FindItemById(this.Context.TileAccessor, this.Context.ContainerManager, typeId, this.Requestor);

                bool successfulDeletion = this.PerformItemDeletion(item, fromCylinder, subIndex);

                if (!successfulDeletion)
                {
                    // handles check for isPlayer.
                    // this.NotifyOfFailure();
                    return;
                }
            });
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
        /// Immediately attempts to perform an item deletion in behalf of the requesting creature, if any.
        /// </summary>
        /// <param name="item">The item being deleted.</param>
        /// <param name="fromCylinder">The cylinder from which the deletion is happening.</param>
        /// <param name="index">Optional. The index within the cylinder from which to delete the item.</param>
        /// <returns>True if the item was successfully deleted, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        private bool PerformItemDeletion(IItem item, ICylinder fromCylinder, byte index = 0xFF)
        {
            if (item == null || fromCylinder == null)
            {
                return false;
            }

            IThing itemAsThing = item as IThing;

            // At this point, we have an item to remove, let's proceed.
            (bool removeSuccessful, IThing remainder) = fromCylinder.RemoveContent(this.Context.ItemFactory, ref itemAsThing, index, amount: item.Amount);

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
                            new TileUpdatedNotificationArguments(
                                fromTile.Location,
                                this.Context.MapDescriptor.DescribeTile)));
            }

            // TODO: do we really need a requestor here?
            this.TriggerSeparationEventRules(new SeparationEventRuleArguments(fromCylinder.Location, item, this.Requestor));

            return true;
        }
    }
}
