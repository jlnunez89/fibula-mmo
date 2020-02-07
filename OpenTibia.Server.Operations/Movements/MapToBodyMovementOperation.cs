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
    using OpenTibia.Server.Operations.Conditions;
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
        /// <param name="context">The context of the operation.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="fromLocation">The location from which the movement is happening.</param>
        /// <param name="toCreature">The creature to which the movement is happening.</param>
        /// <param name="toCreatureSlot">The slot of the creature to which the movement is happening.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        public MapToBodyMovementOperation(
            ILogger logger,
            IOperationContext context,
            uint creatureRequestingId,
            IThing thingMoving,
            Location fromLocation,
            ICreature toCreature,
            Slot toCreatureSlot,
            byte amount = 1)
            : base(logger, context, context?.TileAccessor.GetTileAt(fromLocation), (toCreature as IPlayer)?.Inventory[(byte)toCreatureSlot] as IContainerItem, creatureRequestingId)
        {
            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            this.Conditions.Add(new CanDressThingAtTargetSlotEventCondition(() => toCreature, thingMoving, toCreatureSlot));
            this.Conditions.Add(new ThingCanBeMovedCondition(this.Requestor, thingMoving));
            this.Conditions.Add(new TileContainsThingEventCondition(this.Context.TileAccessor, thingMoving, fromLocation, amount));
            this.Conditions.Add(new RequestorIsInRangeToMoveEventCondition(this.Requestor, () => fromLocation));
            this.Conditions.Add(new LocationsMatchEventCondition(() => thingMoving?.Location ?? default, () => fromLocation));

            this.ActionsOnPass.Add(() =>
            {
                bool moveSuccessful = thingMoving is IItem item &&
                                      toCreature is IPlayer targetPlayer &&
                                      this.Context.TileAccessor.GetTileAt(fromLocation, out ITile fromTile) &&
                                      this.PerformItemMovement(item, fromTile, targetPlayer.Inventory[(byte)toCreatureSlot] as IContainerItem, toIndex: 0, amountToMove: amount, requestorCreature: this.Requestor);

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