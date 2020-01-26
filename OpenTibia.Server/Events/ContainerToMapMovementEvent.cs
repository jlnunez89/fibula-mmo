// -----------------------------------------------------------------
// <copyright file="ContainerToMapMovementEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Events
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Scheduling.Contracts.Enumerations;
    using OpenTibia.Server;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.EventConditions;
    using Serilog;

    /// <summary>
    /// Internal class that represents a movement event that happens from inside a container to the map.
    /// </summary>
    internal class ContainerToMapMovementEvent : BaseMovementEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerToMapMovementEvent"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="game">A reference to the game instance.</param>
        /// <param name="connectionManager">A reference to the connection manager in use.</param>
        /// <param name="tileAccessor">A reference to the tile accessor in use.</param>
        /// <param name="creatureFinder">A reference to the creture finder in use.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the movement.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="fromCreatureId">The id of the creature in which the movement is happening.</param>
        /// <param name="fromCreatureContainerId">The id of the container from which the movement is happening.</param>
        /// <param name="fromCreatureContainerIndex">The index in the container from which the movement is happening.</param>
        /// <param name="toLocation">The location in the map to which the movement is happening.</param>
        /// <param name="amount">Optional. The amount of the thing to move. Must be positive. Defaults to 1.</param>
        /// <param name="evaluationTime">Optional. The evaluation time policy for this event. Defaults to <see cref="EvaluationTime.OnBoth"/>.</param>
        public ContainerToMapMovementEvent(
            ILogger logger,
            IGame game,
            IConnectionManager connectionManager,
            ITileAccessor tileAccessor,
            ICreatureFinder creatureFinder,
            uint creatureRequestingId,
            IThing thingMoving,
            uint fromCreatureId,
            byte fromCreatureContainerId,
            byte fromCreatureContainerIndex,
            Location toLocation,
            byte amount = 1,
            EvaluationTime evaluationTime = EvaluationTime.OnBoth)
            : base(logger, game, connectionManager, creatureFinder, creatureRequestingId, evaluationTime)
        {
            tileAccessor.ThrowIfNull(nameof(tileAccessor));

            if (amount == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(amount));
            }

            this.Conditions.Add(new LocationNotObstructedEventCondition(tileAccessor, this.Requestor, () => thingMoving, () => toLocation));
            this.Conditions.Add(new LocationHasTileWithGroundEventCondition(tileAccessor, () => toLocation));
            this.Conditions.Add(new ContainerIsOpenEventCondition(() => creatureFinder.FindCreatureById(fromCreatureId), fromCreatureContainerId));

            var onPassAction = new GenericEventAction(() =>
            {
                bool moveSuccessful = thingMoving is IItem item &&
                                      creatureFinder.FindCreatureById(fromCreatureId) is IPlayer targetPlayer &&
                                      tileAccessor.GetTileAt(toLocation, out ITile toTile) &&
                                      this.Game.PerformItemMovement(item, targetPlayer.GetContainerById(fromCreatureContainerId), toTile, fromIndex: fromCreatureContainerIndex, amountToMove: amount, requestorCreature: this.Requestor);

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
            });

            this.ActionsOnPass.Add(onPassAction);
        }
    }
}