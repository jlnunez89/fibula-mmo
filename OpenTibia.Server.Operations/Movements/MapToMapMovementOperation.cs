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
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Operations.Actions;
    using OpenTibia.Server.Operations.Conditions;
    using OpenTibia.Server.Operations.Notifications;
    using OpenTibia.Server.Operations.Notifications.Arguments;
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
            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            if (!isTeleport && this.Requestor != null)
            {
                this.Conditions.Add(new CanThrowBetweenEventCondition(this.Context.TileAccessor, this.Requestor, () => fromLocation, () => toLocation));
            }

            if (thingMoving is ICreature creatureMoving)
            {
                // Don't add any conditions if this wasn't a creature requesting, i.e. if the request comes from a script.
                if (!isTeleport && this.Requestor != null)
                {
                    this.Conditions.Add(new LocationNotAvoidEventCondition(this.Context.TileAccessor, this.Requestor, () => creatureMoving, () => toLocation));
                    this.Conditions.Add(new LocationsAreDistantByEventCondition(() => fromLocation, () => toLocation));
                    this.Conditions.Add(new CreatureThrowBetweenFloorsEventCondition(this.Requestor, () => creatureMoving, () => toLocation));
                }
            }

            this.Conditions.Add(new ThingCanBeMovedCondition(this.Requestor, thingMoving));
            this.Conditions.Add(new RequestorIsInRangeToMoveEventCondition(this.Requestor, () => fromLocation));
            this.Conditions.Add(new LocationNotObstructedEventCondition(this.Context.TileAccessor, this.Requestor, () => thingMoving, () => toLocation));
            this.Conditions.Add(new LocationHasTileWithGroundEventCondition(this.Context.TileAccessor, () => toLocation));
            this.Conditions.Add(new UnpassItemsInRangeEventCondition(this.Requestor, () => thingMoving, () => toLocation));
            this.Conditions.Add(new LocationsMatchEventCondition(() => thingMoving?.Location ?? default, () => fromLocation));
            this.Conditions.Add(new TileContainsThingEventCondition(this.Context.TileAccessor, thingMoving, fromLocation, amount));

            this.ActionsOnPass.Add(() =>
            {
                bool moveSuccessful = false;

                if (thingMoving is ICreature creatureMoving)
                {
                    moveSuccessful = this.PerformCreatureMovement(creatureMoving, toLocation, isTeleport, requestorCreature: this.Requestor);
                }
                else if (thingMoving is IItem item)
                {
                    moveSuccessful = this.Context.TileAccessor.GetTileAt(fromLocation, out ITile fromTile) &&
                                     this.Context.TileAccessor.GetTileAt(toLocation, out ITile toTile) &&
                                     this.PerformItemMovement(item, fromTile, toTile, fromStackPos, amountToMove: amount, requestorCreature: this.Requestor);
                }

                if (!moveSuccessful)
                {
                    // handles check for whether there is a player to notify.
                    // this.NotifyOfFailure();
                    return;
                }

                if (this.Requestor is IPlayer player && toLocation != player.Location && player != thingMoving)
                {
                    var directionToDestination = player.Location.DirectionTo(toLocation);

                    this.Context.Scheduler.ScheduleEvent(
                        new TurnToDirectionOperation(this.Logger, this.Context, player, directionToDestination),
                        this.Context.Scheduler.CurrentTime);
                }
            });
        }

        /// <summary>
        /// Gets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; }

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
                this.Context.Scheduler.ImmediateEvent(
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
                foreach (var container in player.OpenContainers)
                {
                    if (container == null)
                    {
                        continue;
                    }

                    var locationDiff = container.Location - player.Location;

                    if ((locationDiff.MaxValueIn2D > 1 || locationDiff.Z != 0) && container.IsTracking(player.Id, out byte containerId))
                    {
                        this.CloseContainer(player, container, containerId);
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