// -----------------------------------------------------------------
// <copyright file="MoveThingHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Handlers.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Operations;
    using OpenTibia.Server.Operations.Arguments;
    using Serilog;

    /// <summary>
    /// Class that represents a handler for a player moving something on the game.
    /// </summary>
    public class MoveThingHandler : GameHandler
    {
        /// <summary>
        /// A value used to delay movements for creatures.
        /// </summary>
        private static readonly TimeSpan CreatureMovementDelay = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveThingHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        public MoveThingHandler(ILogger logger, IGameContext gameContext)
            : base(logger, gameContext)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.ItemThrow;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutgoingPacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            var moveThingInfo = message.ReadMoveThingInfo();

            if (!(this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return null;
            }

            //player.ClearAllLocationBasedOperations();

            this.Context.Scheduler.CancelAllFor(player.Id, typeof(IMovementOperation));

            var movementOperation =
                this.Context.OperationFactory.Create(
                    OperationType.Movement,
                    new MovementOperationCreationArguments(
                        player.Id,
                        moveThingInfo.ThingClientId,
                        moveThingInfo.FromLocation,
                        moveThingInfo.FromStackPos,
                        player.Id,
                        moveThingInfo.ToLocation,
                        player.Id,
                        moveThingInfo.Amount));

            var locationDiff = moveThingInfo.FromLocation - player.Location;

            if (moveThingInfo.FromLocation.Type == LocationType.Map && locationDiff.MaxValueIn2D > 1)
            {
                // Too far away from it, we need to move closer first.
                var directions = this.Context.PathFinder.FindBetween(player.Location, moveThingInfo.FromLocation, out Location retryLocation, onBehalfOfCreature: player, considerAvoidsAsBlock: true);

                if (directions == null || !directions.Any())
                {
                    return new TextMessagePacket(MessageType.StatusSmall, OperationMessage.ThereIsNoWay).YieldSingleItem();
                }
                else
                {
                    //player.SetOperationAtLocation(retryLocation, movementOperation);

                    this.ScheduleNewOperation(
                        this.Context.OperationFactory.Create(
                            OperationType.AutoWalk,
                            new AutoWalkOperationCreationArguments(player.Id, player, directions.ToArray())));
                }

                return null;
            }

            // First we turn towards the thing we're moving.
            if (moveThingInfo.FromLocation.Type == LocationType.Map && player.Location != moveThingInfo.FromLocation)
            {
                var directionToThing = player.Location.DirectionTo(moveThingInfo.FromLocation);

                this.ScheduleNewOperation(
                    this.Context.OperationFactory.Create(
                        OperationType.Turn,
                        new TurnToDirectionOperationCreationArguments(player, directionToThing)));
            }

            this.ScheduleNewOperation(movementOperation, moveThingInfo.ThingClientId == ICreature.CreatureThingId ? CreatureMovementDelay : TimeSpan.Zero);

            return null;
        }
    }
}