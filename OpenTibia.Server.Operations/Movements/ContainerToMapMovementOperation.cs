﻿// -----------------------------------------------------------------
// <copyright file="ContainerToMapMovementOperation.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Operations.Actions;
    using Serilog;

    /// <summary>
    /// public class that represents a movement operation that happens from inside a container to the map.
    /// </summary>
    public class ContainerToMapMovementOperation : BaseMovementOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerToMapMovementOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The context of the operation.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="fromCreature">The creature in which the movement is happening.</param>
        /// <param name="fromCreatureContainerId">The id of the container from which the movement is happening.</param>
        /// <param name="fromCreatureContainerIndex">The index in the container from which the movement is happening.</param>
        /// <param name="toLocation">The location in the map to which the movement is happening.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        public ContainerToMapMovementOperation(
            ILogger logger,
            IOperationContext context,
            uint creatureRequestingId,
            IThing thingMoving,
            ICreature fromCreature,
            byte fromCreatureContainerId,
            byte fromCreatureContainerIndex,
            Location toLocation,
            byte amount = 1)
            : base(logger, context, context.ContainerManager?.FindForCreature(fromCreature.Id, fromCreatureContainerId), context?.TileAccessor.GetTileAt(toLocation), creatureRequestingId)
        {
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

                var destinationTile = this.ToCylinder as ITile;

                var destinationHasGround = destinationTile?.Ground != null;
                var destinationNotObstructed = !destinationTile.BlocksLay && !(item.BlocksPass && destinationTile.BlocksPass);

                var sourceContainer = this.FromCylinder as IContainerItem;
                var creatureHasSourceContainerOpen = sourceContainer != null;
                var canThrowBetweenLocations = this.CanThrowBetweenMapLocations(fromCreature.Location, toLocation, checkLineOfSight: true);

                bool moveSuccessful = destinationHasGround &&
                                      destinationNotObstructed &&
                                      creatureHasSourceContainerOpen &&
                                      canThrowBetweenLocations &&
                                      this.PerformItemMovement(item, sourceContainer, destinationTile, fromIndex: fromCreatureContainerIndex, amountToMove: amount, requestorCreature: this.Requestor);

                if (!moveSuccessful)
                {
                    // handles check for isPlayer.
                    // this.NotifyOfFailure();
                    return;
                }

                if (this.Requestor is IPlayer player && toLocation != player.Location && player != thingMoving)
                {
                    var directionToDestination = player.Location.DirectionTo(toLocation);

                    this.Context.Scheduler.ImmediateEvent(new TurnToDirectionOperation(this.Logger, this.Context, player, directionToDestination));
                }
            });
        }

        /// <summary>
        /// Gets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; }
    }
}