﻿// -----------------------------------------------------------------
// <copyright file="MapToContainerMovementEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.MovementEvents
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Scheduling.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.MovementEvents.EventConditions;
    using Serilog;

    /// <summary>
    /// Internal class that represents a movement event that happens from the map to inside a container.
    /// </summary>
    internal class MapToContainerMovementEvent : BaseMovementEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapToContainerMovementEvent"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="game">A reference to the game instance.</param>
        /// <param name="connectionManager">A reference to the connection manager in use.</param>
        /// <param name="tileAccessor">A reference to the tile accessor in use.</param>
        /// <param name="creatureFinder">A reference to the creture finder in use.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="fromLocation">The location from which the movement is happening.</param>
        /// <param name="toCreatureId">The id of the creature that should known the container to which the movement is happening.</param>
        /// <param name="toCreatureContainerId">The id of the container to which the movement is happening.</param>
        /// <param name="toCreatureContainerIndex">The index in the container to which the movement is happening.</param>
        /// <param name="fromStackPos">Optional. The position in the stack of the location from which the movement is happening. Defaults to <see cref="byte.MaxValue"/>, which makes the system take the top thing at the location.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        /// <param name="evaluationTime">Optional. The evaluation time policy for this event. Defaults to <see cref="EvaluationTime.OnBoth"/>.</param>
        public MapToContainerMovementEvent(
            ILogger logger,
            IGame game,
            IConnectionManager connectionManager,
            ITileAccessor tileAccessor,
            ICreatureFinder creatureFinder,
            uint creatureRequestingId,
            IThing thingMoving,
            Location fromLocation,
            uint toCreatureId,
            byte toCreatureContainerId,
            byte toCreatureContainerIndex,
            byte fromStackPos = byte.MaxValue,
            byte amount = 1,
            EvaluationTime evaluationTime = EvaluationTime.OnBoth)
            : base(logger, game, connectionManager, creatureFinder, creatureRequestingId, evaluationTime)
        {
            tileAccessor.ThrowIfNull(nameof(tileAccessor));

            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            this.Conditions.Add(new RequestorIsInRangeToMoveEventCondition(this.Requestor, () => fromLocation));
            this.Conditions.Add(new LocationNotObstructedEventCondition(tileAccessor, this.Requestor, () => thingMoving, () => toLocation));
            this.Conditions.Add(new LocationHasTileWithGroundEventCondition(tileAccessor, () => toLocation));
            this.Conditions.Add(new UnpassItemsInRangeEventCondition(this.Requestor, () => thingMoving, () => toLocation));
            this.Conditions.Add(new LocationsMatchEventCondition(() => thingMoving?.Location ?? default, () => fromLocation));
            this.Conditions.Add(new TileContainsThingEventCondition(tileAccessor, thingMoving, fromLocation, amount));

            var onPassAction = new GenericEventAction(() =>
            {
                bool moveSuccessful = this.Game.PerformThingMovementBetweenTiles(thingMoving, fromLocation, toLocation, fromStackPos, amount, isTeleport);

                if (!moveSuccessful)
                {
                    // handles check for isPlayer.
                    this.NotifyOfFailure();

                    return;
                }

                if (this.Requestor is IPlayer player && toLocation != player.Location && player != thingMoving)
                {
                    var directionToDestination = player.Location.DirectionTo(toLocation);

                    this.Game.PlayerRequest_TurnToDirection(player, directionToDestination);
                }

                this.Game.EvaluateSeparationEventRules(fromLocation, thingMoving, this.Requestor);

                this.Game.EvaluateCollisionEventRules(toLocation, thingMoving, this.Requestor);
            });

            this.ActionsOnPass.Add(onPassAction);
        }
    }
}