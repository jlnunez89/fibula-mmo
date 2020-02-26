// -----------------------------------------------------------------
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

            this.ThingMoving = thingMoving;
            this.Amount = amount;
            this.FromCreature = fromCreature;
            this.FromCreatureContainerIndex = fromCreatureContainerIndex;
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
        /// Gets the creature from which the movement is happening.
        /// </summary>
        public ICreature FromCreature { get; }

        /// <summary>
        /// Gets the index within the container from which the movement is happening.
        /// </summary>
        public byte FromCreatureContainerIndex { get; }

        /// <summary>
        /// Gets the location to which the movement is happening.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        public override void Execute()
        {
            if (!(this.ThingMoving is IItem item))
            {
                this.SendFailureNotification(OperationMessage.MayNotMoveThis);

                return;
            }

            var destinationTile = this.ToCylinder as ITile;
            var sourceContainer = this.FromCylinder as IContainerItem;

            // Declare some pre-conditions.
            var destinationHasGround = destinationTile?.Ground != null;
            var destinationIsObstructed = destinationTile.BlocksLay || (item.BlocksPass && destinationTile.BlocksPass);
            var creatureHasSourceContainerOpen = sourceContainer != null;
            var canThrowBetweenLocations = this.CanThrowBetweenMapLocations(this.FromCreature.Location, this.ToLocation, checkLineOfSight: true);

            if (!destinationHasGround || !canThrowBetweenLocations)
            {
                this.SendFailureNotification(OperationMessage.MayNotThrowThere);
            }
            else if (destinationIsObstructed)
            {
                this.SendFailureNotification(OperationMessage.NotEnoughRoom);
            }
            else if (!creatureHasSourceContainerOpen)
            {
                this.SendFailureNotification(OperationMessage.MustFirstOpenThatContainer);
            }
            else if (!this.PerformItemMovement(item, sourceContainer, destinationTile, fromIndex: this.FromCreatureContainerIndex, amountToMove: this.Amount, requestorCreature: this.Requestor))
            {
                // Something else went wrong.
                this.SendFailureNotification();
            }
            else if (this.Requestor is IPlayer player && this.ToLocation != player.Location && player != this.ThingMoving)
            {
                var directionToDestination = player.Location.DirectionTo(this.ToLocation);

                this.Context.Scheduler.ScheduleEvent(new TurnToDirectionOperation(this.Logger, this.Context, player, directionToDestination));
            }
        }
    }
}