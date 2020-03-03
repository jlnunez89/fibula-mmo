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
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using Serilog;

    /// <summary>
    /// Class that represents an event for an item change.
    /// </summary>
    public class ChangeItemOperation : BaseActionOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeItemOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The operation context.</param>
        /// <param name="requestorId">The id of the creature requesting the change.</param>
        /// <param name="typeId">The type id of the item being changed.</param>
        /// <param name="fromLocation">The location from which the item is being changed.</param>
        /// <param name="toTypeId">The type id of the item to change to.</param>
        /// <param name="fromStackPos">The position in the stack from which the item is being used.</param>
        /// <param name="index">The index of the item being used.</param>
        /// <param name="carrierCreature">The creature who is carrying the thing, if any.</param>
        public ChangeItemOperation(
            ILogger logger,
            IOperationContext context,
            uint requestorId,
            ushort typeId,
            Location fromLocation,
            ushort toTypeId,
            byte fromStackPos = byte.MaxValue,
            byte index = 1,
            ICreature carrierCreature = null)
            : base(logger, context, requestorId)
        {
            this.FromCylinder = fromLocation.GetCyclinder(this.Context.TileAccessor, this.Context.ContainerManager, ref fromStackPos, ref index, carrierCreature);
            this.Item = fromLocation.FindItemById(this.Context.TileAccessor, this.Context.ContainerManager, typeId, carrierCreature);
            this.Index = index;
            this.ToTypeId = toTypeId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeItemOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The operation context.</param>
        /// <param name="requestorId">The id of the creature requesting the change.</param>
        /// <param name="item">The item being changed.</param>
        /// <param name="fromCylinder">The cylinder from which the item is being changed.</param>
        /// <param name="index">The index of the item being changed.</param>
        /// <param name="toTypeId">The type id of the item to change to.</param>
        public ChangeItemOperation(ILogger logger, IOperationContext context, uint requestorId, IItem item, ICylinder fromCylinder, byte index, ushort toTypeId)
            : base(logger, context, requestorId)
        {
            this.Item = item;
            this.FromCylinder = fromCylinder;
            this.Index = index;
            this.ToTypeId = toTypeId;
        }

        /// <summary>
        /// Gets a reference to the cylinder from which the item is being changed.
        /// </summary>
        public ICylinder FromCylinder { get; }

        /// <summary>
        /// Gets the item being changed.
        /// </summary>
        public IItem Item { get; }

        /// <summary>
        /// Gets the index of the item being used.
        /// </summary>
        public byte Index { get; }

        /// <summary>
        /// Gets the type id of the item to change to.
        /// </summary>
        public ushort ToTypeId { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        public override void Execute()
        {
            const byte FallbackIndex = 0xFF;

            if (this.Item == null || this.FromCylinder == null)
            {
                return;
            }

            IThing newItem = this.Context.ItemFactory.Create(this.ToTypeId);

            if (newItem == null)
            {
                return;
            }

            // At this point, we have an item to change, and we were able to generate the new one, let's proceed.
            (bool replaceSuccessful, IThing replaceRemainder) = this.FromCylinder.ReplaceContent(this.Context.ItemFactory, this.Item, newItem, this.Index, this.Item.Amount);

            if (!replaceSuccessful || replaceRemainder != null)
            {
                this.AddContentToCylinderChain(this.FromCylinder.GetCylinderHierarchy(), FallbackIndex, ref replaceRemainder, this.Requestor);
            }

            if (replaceSuccessful)
            {
                if (this.FromCylinder is ITile atTile)
                {
                    this.Context.Scheduler.ScheduleEvent(
                            new TileUpdatedNotification(
                            this.Logger,
                            this.Context.CreatureFinder,
                            () => this.Context.ConnectionFinder.PlayersThatCanSee(this.Context.CreatureFinder, atTile.Location),
                            new TileUpdatedNotificationArguments(atTile.Location, this.Context.MapDescriptor.DescribeTile)));

                    this.TriggerCollisionEventRules(new CollisionEventRuleArguments(this.FromCylinder.Location, this.Item, this.Requestor));
                }
            }
        }
    }
}
