// -----------------------------------------------------------------
// <copyright file="MapToContainerMovementOperation.cs" company="2Dudes">
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
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using Serilog;

    /// <summary>
    /// public class that represents a movement operation that happens from the map to inside a container.
    /// </summary>
    public class MapToContainerMovementOperation : BaseMovementOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapToContainerMovementOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The context of the operation.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="fromLocation">The location from which the movement is happening.</param>
        /// <param name="toCreature">The creature that should known the container to which the movement is happening.</param>
        /// <param name="toCreatureContainerId">The id of the container to which the movement is happening.</param>
        /// <param name="toCreatureContainerIndex">The index in the container to which the movement is happening.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        public MapToContainerMovementOperation(
            ILogger logger,
            IOperationContext context,
            uint creatureRequestingId,
            IThing thingMoving,
            Location fromLocation,
            ICreature toCreature,
            byte toCreatureContainerId,
            byte toCreatureContainerIndex,
            byte amount = 1)
            : base(logger, context, context?.TileAccessor.GetTileAt(fromLocation), context?.ContainerManager?.FindForCreature(toCreature.Id, toCreatureContainerId), creatureRequestingId)
        {
            thingMoving.ThrowIfNull(nameof(thingMoving));

            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            this.ActionsOnPass.Add(() =>
            {
                if (!(thingMoving is IItem item))
                {
                    // You may not move this.
                    return;
                }

                var sourceTile = this.FromCylinder as ITile;
                var itemStackPos = sourceTile?.GetStackPositionOfThing(item);

                var sourceTileNotNull = sourceTile != null;
                var thingCanBeMoved = thingMoving.CanBeMoved || thingMoving == this.Requestor;
                var locationsMatch = thingMoving?.Location == fromLocation;
                var requestorInRange = this.Requestor == null || (this.Requestor.Location - fromLocation).MaxValueIn2D <= 1;
                var sourceTileHasEnoughItemAmount = itemStackPos != byte.MaxValue &&
                                                    sourceTile.GetTopThingByOrder(this.Context.CreatureFinder, itemStackPos.Value) == item &&
                                                    item.Amount >= amount;

                var destinationContainer = this.ToCylinder as IContainerItem;
                var creatureHasDestinationContainerOpen = destinationContainer != null;

                bool moveSuccessful = sourceTileNotNull &&
                                      thingCanBeMoved &&
                                      locationsMatch &&
                                      sourceTileHasEnoughItemAmount &&
                                      requestorInRange &&
                                      creatureHasDestinationContainerOpen &&
                                      this.PerformItemMovement(item, sourceTile, destinationContainer, toIndex: toCreatureContainerIndex, amountToMove: amount, requestorCreature: this.Requestor);

                if (!moveSuccessful)
                {
                    // handles check for isPlayer.
                    // this.NotifyOfFailure();
                    return;
                }
            });
        }

        /// <summary>
        /// Gets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; }
    }
}