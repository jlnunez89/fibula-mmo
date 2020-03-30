// -----------------------------------------------------------------
// <copyright file="BodyToMapMovementOperation.cs" company="2Dudes">
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
    using OpenTibia.Server.Operations.Actions;
    using Serilog;

    /// <summary>
    /// public class that represents a movement operation that happens from a player's inventory to the map.
    /// </summary>
    public class BodyToMapMovementOperation : BaseMovementOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BodyToMapMovementOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="fromCreature">The creature from which the movement is happening.</param>
        /// <param name="fromCreatureSlot">The slot of the creature from which the movement is happening.</param>
        /// <param name="toLocation">The location in the map to which the movement is happening.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        public BodyToMapMovementOperation(
            ILogger logger,
            uint creatureRequestingId,
            IThing thingMoving,
            ICreature fromCreature,
            Slot fromCreatureSlot,
            Location toLocation,
            byte amount = 1)
            : base(logger, fromCreature?.Inventory[(byte)fromCreatureSlot] as IContainerItem, context?.TileAccessor.GetTileAt(toLocation), creatureRequestingId)
        {
            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            this.ThingMoving = thingMoving;
            this.Amount = amount;
            this.FromCreature = fromCreature;
            this.ToLocation = toLocation;
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
        /// Gets the location to which the movement is happening.
        /// </summary>
        public Location ToLocation { get; }

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

            var destinationTile = this.ToCylinder as ITile;

            // Declare some pre-conditions.
            var destinationHasGround = destinationTile?.Ground != null;
            var destinationIsObstructed = destinationTile.BlocksLay || (item.BlocksPass && destinationTile.BlocksPass);
            var canThrowBetweenLocations = this.CanThrowBetweenMapLocations(context.TileAccessor, this.FromCreature.Location, this.ToLocation, checkLineOfSight: true);

            if (!destinationHasGround || !canThrowBetweenLocations)
            {
                this.DispatchTextNotification(context, OperationMessage.MayNotThrowThere);
            }
            else if (destinationIsObstructed)
            {
                this.DispatchTextNotification(context, OperationMessage.NotEnoughRoom);
            }
            else if (!this.PerformItemMovement(context, item, this.FromCylinder, destinationTile, 0, amountToMove: this.Amount, requestorCreature: this.GetRequestor(context.CreatureFinder)))
            {
                // Something else went wrong.
                this.DispatchTextNotification(context);
            }
            else if (this.GetRequestor(context.CreatureFinder) is IPlayer player && this.ToLocation != player.Location && player != this.ThingMoving)
            {
                var directionToDestination = player.Location.DirectionTo(this.ToLocation);

                context.Scheduler.ScheduleEvent(new TurnToDirectionOperation(this.Logger, player, directionToDestination));
            }
        }
    }
}