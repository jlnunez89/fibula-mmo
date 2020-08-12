// -----------------------------------------------------------------
// <copyright file="RotateItemHandler.cs" company="2Dudes">
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
    /// Class that represents an item rotation handler for the game server.
    /// </summary>
    public class RotateItemHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateItemHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        public RotateItemHandler(ILogger logger, IGameContext gameContext)
            : base(logger, gameContext)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForRequestType => (byte)GameRequestType.ItemRotate;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IResponsePacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IResponsePacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            var itemRotationInfo = message.ReadItemRotationInfo();

            if (!(this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return null;
            }

            // Before actually using the item, check if we're close enough to use it.
            if (itemRotationInfo.AtLocation.Type == LocationType.Map)
            {
                // Turn to the item if it's not exactly the location.
                if (player.Location != itemRotationInfo.AtLocation)
                {
                    var directionToThing = player.Location.DirectionTo(itemRotationInfo.AtLocation);

                    this.ScheduleNewOperation(
                        this.Context.OperationFactory.Create(
                            OperationType.Turn,
                            new TurnToDirectionOperationCreationArguments(player, directionToThing)));
                }

                var locationDiff = itemRotationInfo.AtLocation - player.Location;

                if (locationDiff.Z != 0)
                {
                    // it's on a different floor...
                    return new TextMessagePacket(MessageType.StatusSmall, OperationMessage.ThereIsNoWay).YieldSingleItem();
                }
            }

            return this.RotateItemAt(player, itemRotationInfo.AtLocation, itemRotationInfo.Index, itemRotationInfo.ItemClientId);
        }

        /// <summary>
        /// Attempts to rotate an item.
        /// </summary>
        /// <param name="creature">The creature requesting the rotation.</param>
        /// <param name="atLocation">The location at which the item to rotate is.</param>
        /// <param name="index">The index where the item to rotate is.</param>
        /// <param name="typeId">The type id of the item to rotate.</param>
        private IEnumerable<IResponsePacket> RotateItemAt(ICreature creature, Location atLocation, byte index, ushort typeId)
        {
            if (this.Context.TileAccessor.GetTileAt(atLocation, out ITile targetTile))
            {
                if (!(targetTile.GetTopThingByOrder(this.Context.CreatureFinder, index) is IItem item) || item.ThingId != typeId || !item.CanBeRotated)
                {
                    return null;
                }

                var locationDiff = atLocation - creature.Location;

                var changeItemOperation =
                        this.Context.OperationFactory.Create(
                            OperationType.ChangeItem,
                            new ChangeItemOperationCreationArguments(requestorId: 0, item.ThingId, atLocation, item.RotateTo));

                this.Context.EventRulesApi.ClearAllFor(changeItemOperation.GetPartitionKey());
                this.Context.Scheduler.CancelAllFor(creature.Id, typeof(IMovementOperation));

                if (atLocation.Type == LocationType.Map && locationDiff.MaxValueIn2D > 1)
                {
                    // Too far away from it, we need to move closer first.
                    var directions = this.Context.PathFinder.FindBetween(creature.Location, atLocation, out Location retryLocation, onBehalfOfCreature: creature, considerAvoidsAsBlocking: true);

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

                        this.Context.EventRulesApi.SetupRule(new ExpediteOperationMovementEventRule(this.Logger, changeItemOperation, conditionsForExpedition, 1), changeItemOperation.GetPartitionKey());

                        this.ScheduleNewOperation(
                            this.Context.OperationFactory.Create(
                                OperationType.AutoWalk,
                                new AutoWalkOperationCreationArguments(creature.Id, creature, directions.ToArray())));
                    }

                    return null;
                }

                this.ScheduleNewOperation(changeItemOperation);
            }

            return null;
        }
    }
}