// -----------------------------------------------------------------
// <copyright file="OnMapMovementEvent.cs" company="2Dudes">
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
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Scheduling.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.MovementEvents.EventConditions;
    using Serilog;

    /// <summary>
    /// Internal class that represents a movement event that happens on the map.
    /// </summary>
    internal class OnMapMovementEvent : BaseMovementEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnMapMovementEvent"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="game"></param>
        /// <param name="connectionManager"></param>
        /// <param name="tileAccessor"></param>
        /// <param name="creatureFinder"></param>
        /// <param name="itemFactory"></param>
        /// <param name="creatureRequestingId"></param>
        /// <param name="thingMoving"></param>
        /// <param name="fromLocation"></param>
        /// <param name="fromStackPos"></param>
        /// <param name="toLocation"></param>
        /// <param name="count"></param>
        /// <param name="isTeleport"></param>
        public OnMapMovementEvent(
            ILogger logger,
            IGame game,
            IConnectionManager connectionManager,
            ITileAccessor tileAccessor,
            ICreatureFinder creatureFinder,
            uint creatureRequestingId,
            IThing thingMoving,
            Location fromLocation,
            byte fromStackPos,
            Location toLocation,
            byte count = 1,
            bool isTeleport = false)
            : base(logger, game, connectionManager, creatureFinder, creatureRequestingId, EvaluationTime.OnBoth)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
            }

            if (!isTeleport && this.Requestor != null)
            {
                this.Conditions.Add(new CanThrowBetweenEventCondition(creatureFinder, game, this.RequestorId, fromLocation, toLocation));
            }

            this.Conditions.Add(new RequestorIsInRangeToMoveEventCondition(creatureFinder, this.RequestorId, fromLocation));
            this.Conditions.Add(new LocationNotObstructedEventCondition(creatureFinder, tileAccessor, this.RequestorId, thingMoving, toLocation));
            this.Conditions.Add(new LocationHasTileWithGroundEventCondition(tileAccessor, toLocation));
            this.Conditions.Add(new UnpassItemsInRangeEventCondition(creatureFinder, this.RequestorId, thingMoving, toLocation));
            this.Conditions.Add(new LocationsMatchEventCondition(thingMoving?.Location ?? default, fromLocation));
            this.Conditions.Add(new TileContainsThingEventCondition(tileAccessor, thingMoving, fromLocation, count));

            this.ActionsOnPass.Add(new GenericEventAction(() =>
            {
                bool moveSuccessful = game.PerformThingMovementBetweenTiles(thingMoving, fromLocation, fromStackPos, toLocation, count, isTeleport);

                if (moveSuccessful && this.Requestor is IPlayer player && player != thingMoving)
                {
                    var directionToDestination = player.Location.DirectionTo(toLocation);

                    game.PlayerRequest_TurnToDirection(player, directionToDestination);
                }
            }));
        }
    }
}