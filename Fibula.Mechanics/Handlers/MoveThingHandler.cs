// -----------------------------------------------------------------
// <copyright file="MoveThingHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Mechanics.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
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
        public override byte ForRequestType => (byte)GameRequestType.MoveThing;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IResponsePacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IResponsePacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            var moveThingInfo = message.ReadMoveThingInfo();

            if (!(this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return null;
            }

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

            this.Context.EventRulesApi.ClearAllFor(movementOperation.GetPartitionKey());
            this.Context.Scheduler.CancelAllFor(player.Id, typeof(IMovementOperation));

            var locationDiff = moveThingInfo.FromLocation - player.Location;

            if (moveThingInfo.FromLocation.Type == LocationType.Map && locationDiff.MaxValueIn2D > 1)
            {
                // Too far away from it, we need to move closer first.
                var directions = this.Context.PathFinder.FindBetween(player.Location, moveThingInfo.FromLocation, out Location retryLocation, onBehalfOfCreature: player, considerAvoidsAsBlocking: true);

                if (directions == null || !directions.Any())
                {
                    return new TextMessagePacket(MessageType.StatusSmall, OperationMessage.ThereIsNoWay).YieldSingleItem();
                }
                else
                {
                    var conditionsForExpedition = new Func<IEventRuleContext, bool>[]
                    {
                        (context) =>
                        {
                            if (!(context.Arguments is MovementEventRuleArguments movementEventRuleArguments) ||
                                !(movementEventRuleArguments.ThingMoving is ICreature creature))
                            {
                                return false;
                            }

                            return creature.Location == retryLocation;
                        },
                    };

                    this.Context.EventRulesApi.SetupRule(new ExpediteOperationMovementEventRule(this.Logger, movementOperation, conditionsForExpedition, 1), movementOperation.GetPartitionKey());

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