// -----------------------------------------------------------------
// <copyright file="OnMapCreatureMovementEvent.cs" company="2Dudes">
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
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Scheduling.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.MovementEvents.EventConditions;
    using Serilog;

    /// <summary>
    /// Internal class that represents a creature movement event that happens on the map.
    /// </summary>
    internal class OnMapCreatureMovementEvent : OnMapMovementEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnMapCreatureMovementEvent"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="game"></param>
        /// <param name="connectionManager"></param>
        /// <param name="tileAccessor"></param>
        /// <param name="creatureFinder"></param>
        /// <param name="itemFactory"></param>
        /// <param name="requestorId"></param>
        /// <param name="creatureMoving"></param>
        /// <param name="fromLocation"></param>
        /// <param name="toLocation"></param>
        /// <param name="fromStackPos">Optional. The position in the stack of the tile from which the movement is being performed. Defaults to <see cref="byte.MaxValue"/> which signals to attempt to find the thing from the source location.</param>
        /// <param name="isTeleport"></param>
        /// <param name="deferEvaluation"></param>
        public OnMapCreatureMovementEvent(
            ILogger logger,
            IGame game,
            IConnectionManager connectionManager,
            ITileAccessor tileAccessor,
            ICreatureFinder creatureFinder,
            uint requestorId,
            ICreature creatureMoving,
            Location fromLocation,
            Location toLocation,
            byte fromStackPos = byte.MaxValue,
            bool isTeleport = false,
            bool deferEvaluation = false)
            : base(logger, game, connectionManager, tileAccessor, creatureFinder, requestorId, creatureMoving, fromLocation, toLocation, fromStackPos, 1, isTeleport, deferEvaluation ? EvaluationTime.OnExecute : EvaluationTime.OnBoth)
        {
            // Don't add any conditions if this wasn't a creature requesting, i.e. if the request comes from a script.
            if (!isTeleport && this.Requestor != null)
            {
                this.Conditions.Add(new LocationNotAvoidEventCondition(tileAccessor, this.Requestor, () => creatureMoving, () => toLocation));
                this.Conditions.Add(new LocationsAreDistantByEventCondition(() => fromLocation, () => toLocation));
                this.Conditions.Add(new CreatureThrowBetweenFloorsEventCondition(this.Requestor, () => creatureMoving, () => toLocation));
            }
        }
    }
}