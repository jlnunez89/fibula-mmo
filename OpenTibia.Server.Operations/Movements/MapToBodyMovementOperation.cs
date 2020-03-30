// -----------------------------------------------------------------
// <copyright file="MapToBodyMovementOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Movements
{
    using System;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using Serilog;

    /// <summary>
    /// public class that represents a movement operation that happens from the map to the player's inventory.
    /// </summary>
    public class MapToBodyMovementOperation : BaseMovementOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapToBodyMovementOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="fromLocation">The location from which the movement is happening.</param>
        /// <param name="toCreature">The creature to which the movement is happening.</param>
        /// <param name="toCreatureSlot">The slot of the creature to which the movement is happening.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        public MapToBodyMovementOperation(
            ILogger logger,
            uint creatureRequestingId,
            IThing thingMoving,
            Location fromLocation,
            ICreature toCreature,
            Slot toCreatureSlot,
            byte amount = 1)
            : base(logger, context?.TileAccessor.GetTileAt(fromLocation), toCreature?.Inventory[(byte)toCreatureSlot] as IContainerItem, creatureRequestingId)
        {
            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            this.ThingMoving = thingMoving;
            this.Amount = amount;
            this.FromLocation = fromLocation;
        }

        /// <summary>
        /// Gets a reference to the thing moving.
        /// </summary>
        public IThing ThingMoving { get; }

        /// <summary>
        /// Gets the amount of the thing moving.
        /// </summary>
        public byte Amount { get; }

        /// <summary>
        /// Gets the location from which the movement is happening.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IOperationContext context)
        {
            if (!(this.ThingMoving is IItem item))
            {
                this.DispatchTextNotification(context, OperationMessage.MayNotMoveThis);

                return;
            }

            var sourceTile = this.FromCylinder as ITile;
            var itemStackPos = sourceTile?.GetStackPositionOfThing(item);
            var requestor = this.GetRequestor(context.CreatureFinder);

            // Declare some pre-conditions.
            var sourceTileIsNull = sourceTile == null;
            var thingCanBeMoved = this.ThingMoving.CanBeMoved || this.ThingMoving == requestor;
            var locationsMatch = this.ThingMoving?.Location == this.FromLocation;
            var requestorInRange = requestor == null || (requestor.Location - this.FromLocation).MaxValueIn2D <= 1;
            var sourceTileHasEnoughItemAmount = itemStackPos != byte.MaxValue &&
                                                sourceTile.GetTopThingByOrder(context.CreatureFinder, itemStackPos.Value) == item &&
                                                item.Amount >= this.Amount;

            if (sourceTileIsNull || !thingCanBeMoved)
            {
                this.DispatchTextNotification(context, OperationMessage.MayNotMoveThis);
            }
            else if (!locationsMatch)
            {
                // Silent fail.
                return;
            }
            else if (!sourceTileHasEnoughItemAmount)
            {
                this.DispatchTextNotification(context, OperationMessage.NotEnoughQuantity);
            }
            else if (!requestorInRange)
            {
                this.DispatchTextNotification(context, OperationMessage.TooFarAway);
            }
            else if (!this.PerformItemMovement(context, item, sourceTile, this.ToCylinder, toIndex: 0, amountToMove: this.Amount, requestorCreature: requestor))
            {
                // Something else went wrong.
                this.DispatchTextNotification(context);
            }
        }
    }
}