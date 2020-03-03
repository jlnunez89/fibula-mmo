// -----------------------------------------------------------------
// <copyright file="MapToMapMovementOperation.cs" company="2Dudes">
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
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using OpenTibia.Server.Operations.Actions;
    using Serilog;

    /// <summary>
    /// public class that represents a movement operation that happens from and to the map.
    /// </summary>
    public class MapToMapMovementOperation : BaseMovementOperation
    {
        private const int DefaultGroundMovementPenaltyInMs = 200;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapToMapMovementOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The operation's context.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="fromLocation">The location in the map from which the movement is happening.</param>
        /// <param name="toLocation">The location in the map to which the movement is happening.</param>
        /// <param name="fromStackPos">Optional. The position in the stack of the location from which the movement is happening. Defaults to <see cref="byte.MaxValue"/>, which makes the system take the top thing at the location.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        /// <param name="isTeleport">Optional. A value indicating whether the movement is considered a teleportation. Defaults to false.</param>
        public MapToMapMovementOperation(
            ILogger logger,
            IOperationContext context,
            uint creatureRequestingId,
            IThing thingMoving,
            Location fromLocation,
            Location toLocation,
            byte fromStackPos = byte.MaxValue,
            byte amount = 1,
            bool isTeleport = false)
            : base(logger, context, context?.TileAccessor.GetTileAt(fromLocation), context?.TileAccessor.GetTileAt(toLocation), creatureRequestingId)
        {
            thingMoving.ThrowIfNull(nameof(thingMoving));

            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            this.ThingMoving = thingMoving;
            this.Amount = amount;
            this.FromLocation = fromLocation;
            this.FromStackPos = fromStackPos;
            this.ToLocation = toLocation;
            this.IsTeleport = isTeleport;
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
        /// Gets the position in the stack of the location from which the movement is happening.
        /// </summary>
        public byte FromStackPos { get; }

        /// <summary>
        /// Gets the location to which the movement is happening.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Gets a value indicating whether the movement is considered a teleportation.
        /// </summary>
        public bool IsTeleport { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        public override void Execute()
        {
            var sourceTile = this.FromCylinder as ITile;
            var destinationTile = this.ToCylinder as ITile;

            // Declare some pre-conditions.
            var sourceTileIsNull = sourceTile == null;
            var destinationHasGround = destinationTile?.Ground != null;
            var thingCanBeMoved = this.ThingMoving.CanBeMoved || this.ThingMoving == this.Requestor;
            var locationsMatch = this.ThingMoving?.Location == this.FromLocation;
            var requestorInRange = this.Requestor == null || (this.Requestor.Location - this.FromLocation).MaxValueIn2D <= 1;
            var canThrowBetweenLocations = this.IsTeleport || this.Requestor == null || this.CanThrowBetweenMapLocations(this.FromLocation, this.ToLocation, checkLineOfSight: true);

            if (sourceTileIsNull || !thingCanBeMoved)
            {
                this.SendFailureNotification(OperationMessage.MayNotMoveThis);
            }
            else if (!locationsMatch)
            {
                // Silent fail.
                return;
            }
            else if (!destinationHasGround || !canThrowBetweenLocations)
            {
                this.SendFailureNotification(OperationMessage.MayNotThrowThere);
            }
            else if (this.ThingMoving is ICreature creature)
            {
                var distanceBetweenLocations = this.FromLocation - this.ToLocation;
                var creatureStackPos = sourceTile?.GetStackPositionOfThing(creature);

                // More pre-conditions.
                var canThrowThatFar = this.IsTeleport || this.Requestor == null || (distanceBetweenLocations.MaxValueIn2D <= 1 && distanceBetweenLocations.Z == 0);
                var creatureAvoidsDestination = !this.IsTeleport && this.Requestor != null && this.Requestor != creature && destinationTile.IsPathBlocking(/*this.Requestor.DamageTypesToAvoid*/);
                var destinationIsObstructed = !this.IsTeleport && distanceBetweenLocations.Z == 0 && (destinationTile.BlocksLay || destinationTile.BlocksPass);
                var sourceTileHasThing = creatureStackPos != byte.MaxValue &&
                                         sourceTile.GetTopThingByOrder(this.Context.CreatureFinder, creatureStackPos.Value) is ICreature &&
                                         this.Amount == 1;

                if (!sourceTileHasThing)
                {
                    // Silent fail.
                    return;
                }
                else if (creatureAvoidsDestination)
                {
                    this.SendFailureNotification(OperationMessage.NotEnoughRoom);
                }
                else if (destinationIsObstructed)
                {
                    if (this.Requestor != null && creature.Id == this.RequestorId && creature is IPlayer player)
                    {
                        var cancelMovementNotification = new GenericNotification(
                            this.Logger,
                            () => this.Context.ConnectionFinder.FindByPlayerId(player.Id).YieldSingleItem(),
                            new GenericNotificationArguments(new PlayerWalkCancelPacket(player.Direction.GetClientSafeDirection())));

                        this.Context.Scheduler.ScheduleEvent(cancelMovementNotification);
                    }

                    this.SendFailureNotification(OperationMessage.NotEnoughRoom);
                }
                else if (!requestorInRange)
                {
                    this.SendFailureNotification(OperationMessage.TooFarAway);
                }
                else if (!canThrowThatFar)
                {
                    this.SendFailureNotification(OperationMessage.DestinationTooFarAway);
                }
                else if (!this.PerformCreatureMovement(creature, this.ToLocation, this.IsTeleport, requestorCreature: this.Requestor))
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
            else if (this.ThingMoving is IItem item)
            {
                var itemStackPos = sourceTile?.GetStackPositionOfThing(item);
                var distanceBetweenLocations = (this.Requestor?.Location ?? this.FromLocation) - this.ToLocation;

                // More pre-conditions.
                var itemCanBeMoved = item.CanBeMoved;
                var sourceTileHasEnoughItemAmount = itemStackPos != byte.MaxValue &&
                                                    sourceTile.GetTopThingByOrder(this.Context.CreatureFinder, itemStackPos.Value) == item &&
                                                    item.Amount >= this.Amount;
                var destinationIsObstructed = destinationTile.BlocksLay || (item.BlocksPass && destinationTile.BlocksPass);
                var movementInRange = this.Requestor == null || !item.Type.Flags.Contains(ItemFlag.Unpass) || (distanceBetweenLocations.MaxValueIn2D <= 2 && distanceBetweenLocations.Z == 0);

                if (!itemCanBeMoved)
                {
                    this.SendFailureNotification(OperationMessage.MayNotMoveThis);
                }
                else if (destinationIsObstructed)
                {
                    this.SendFailureNotification(OperationMessage.MayNotThrowThere);
                }
                else if (!movementInRange)
                {
                    this.SendFailureNotification(OperationMessage.DestinationTooFarAway);
                }
                else if (!sourceTileHasEnoughItemAmount)
                {
                    this.SendFailureNotification(OperationMessage.NotEnoughQuantity);
                }
                else if (!this.PerformItemMovement(item, sourceTile, destinationTile, this.FromStackPos, amountToMove: this.Amount, requestorCreature: this.Requestor))
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

        /// <summary>
        /// Immediately attempts to perform a creature movement to a tile on the map.
        /// </summary>
        /// <param name="creature">The creature being moved.</param>
        /// <param name="toLocation">The tile to which the movement is being performed.</param>
        /// <param name="isTeleport">Optional. A value indicating whether the movement is considered a teleportation. Defaults to false.</param>
        /// <param name="requestorCreature">Optional. The creature that this movement is being performed in behalf of, if any.</param>
        /// <returns>True if the movement was successfully performed, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        private bool PerformCreatureMovement(ICreature creature, Location toLocation, bool isTeleport = false, ICreature requestorCreature = null)
        {
            if (creature == null || !(creature.ParentCylinder is ITile fromTile) || !this.Context.TileAccessor.GetTileAt(toLocation, out ITile toTile))
            {
                return false;
            }

            var moveDirection = fromTile.Location.DirectionTo(toLocation, true);

            // Try to figure out the position in the stack of the creature.
            var fromTileStackPos = fromTile.GetStackPositionOfThing(creature);

            if (fromTileStackPos == byte.MaxValue)
            {
                // couldn't find this creature in this tile...
                return false;
            }

            IThing creatureAsThing = creature as IThing;

            // Do the actual move first.
            (bool removeSuccessful, IThing removeRemainder) = fromTile.RemoveContent(this.Context.ItemFactory, ref creatureAsThing);

            if (!removeSuccessful)
            {
                return false;
            }

            (bool addSuccessful, IThing addRemainder) = toTile.AddContent(this.Context.ItemFactory, creature);

            if (!addSuccessful)
            {
                // attempt to rollback state.
                (bool rollbackSuccessful, IThing rollbackRemainder) = fromTile.RemoveContent(this.Context.ItemFactory, ref creatureAsThing);

                if (!rollbackSuccessful)
                {
                    // Leaves us in a really bad spot.
                    throw new ApplicationException("Unable to rollback state after filing to move creature. Game state is altered and inconsistent now.");
                }
            }

            var toStackPosition = toTile.GetStackPositionOfThing(creature);

            // Then deal with the consequences of the move.
            creature.TurnToDirection(moveDirection.GetClientSafeDirection());

            var stepDurationTime = this.CalculateStepDuration(creature, moveDirection, fromTile);

            creature.AddExhaustion(this.ExhaustionType, this.Context.Scheduler.CurrentTime, stepDurationTime);

            if (toStackPosition != byte.MaxValue)
            {
                this.Context.Scheduler.ScheduleEvent(
                    new CreatureMovedNotification(
                        this.Logger,
                        this.Context.MapDescriptor,
                        this.Context.CreatureFinder,
                        () => this.Context.ConnectionFinder.PlayersThatCanSee(this.Context.CreatureFinder, fromTile.Location, toLocation),
                        new CreatureMovedNotificationArguments(creature.Id, fromTile.Location, fromTileStackPos, toTile.Location, toStackPosition, isTeleport)));
            }

            if (creature is IPlayer player)
            {
                // If the creature is a player, we must check if the movement caused it to walk away from any open containers.
                foreach (var container in this.Context.ContainerManager.FindAllForCreature(player.Id))
                {
                    if (container == null)
                    {
                        continue;
                    }

                    var locationDiff = container.Location - player.Location;

                    if (locationDiff.MaxValueIn2D > 1 || locationDiff.Z != 0)
                    {
                        var containerId = this.Context.ContainerManager.FindForCreature(player.Id, container);

                        if (containerId != IContainerManager.UnsetContainerPosition)
                        {
                            this.Context.ContainerManager.CloseContainer(player, container, containerId);
                        }
                    }
                }
            }

            this.TriggerSeparationEventRules(new SeparationEventRuleArguments(fromTile.Location, creature, requestorCreature));
            this.TriggerCollisionEventRules(new CollisionEventRuleArguments(toTile.Location, creature, requestorCreature));

            creature.EvaluateLocationBasedActions();
            creature.EvaluateCreatureRangeBasedActions(this.Context.CreatureFinder);

            if (creature is ICombatant combatant)
            {
                // If the creature is a combatant, we must check if the movement caused it to walk into the range of any other combatant attacking it.
                foreach (var attackerId in combatant.AttackedBy)
                {
                    if (!(this.Context.CreatureFinder.FindCreatureById(attackerId) is ICombatant otherCombatant))
                    {
                        continue;
                    }

                    otherCombatant.EvaluateCreatureRangeBasedActions(this.Context.CreatureFinder);
                }
            }

            return true;
        }

        /// <summary>
        /// Calculates the step duration of a creature moving from a given tile in the given direction.
        /// </summary>
        /// <param name="creature">The creature that's moving.</param>
        /// <param name="stepDirection">The direction of the step.</param>
        /// <param name="fromTile">The tile which the creature is moving from.</param>
        /// <returns>The duration time of the step.</returns>
        private TimeSpan CalculateStepDuration(ICreature creature, Direction stepDirection, ITile fromTile)
        {
            if (creature == null)
            {
                return TimeSpan.Zero;
            }

            var tilePenalty = fromTile?.Ground?.MovementPenalty ?? DefaultGroundMovementPenaltyInMs;

            var totalPenalty = tilePenalty * (stepDirection.IsDiagonal() ? 2 : 1);

            var durationInMs = Math.Ceiling(1000 * totalPenalty / (double)Math.Max(1u, creature.Speed) / 50) * 50;

            return TimeSpan.FromMilliseconds(durationInMs);
        }
    }
}