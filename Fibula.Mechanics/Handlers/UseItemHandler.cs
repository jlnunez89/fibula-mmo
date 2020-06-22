// -----------------------------------------------------------------
// <copyright file="UseItemHandler.cs" company="2Dudes">
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
    /// Class that represents an "item use" handler for the game server.
    /// </summary>
    public class UseItemHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        public UseItemHandler(ILogger logger, IGameContext gameContext)
            : base(logger, gameContext)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForRequestType => (byte)GameRequestType.ItemUse;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IResponsePacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IResponsePacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            var itemUseInfo = message.ReadItemUseInfo();

            if (!(this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return null;
            }

            // Before actually using the item, check if we're close enough to use it.
            if (itemUseInfo.FromLocation.Type == LocationType.Map)
            {
                // Turn to the item if it's not exactly the location.
                if (player.Location != itemUseInfo.FromLocation)
                {
                    var directionToThing = player.Location.DirectionTo(itemUseInfo.FromLocation);

                    this.ScheduleNewOperation(
                        this.Context.OperationFactory.Create(
                            OperationType.Turn,
                            new TurnToDirectionOperationCreationArguments(player, directionToThing)));
                }

                var locationDiff = itemUseInfo.FromLocation - player.Location;

                if (locationDiff.Z != 0)
                {
                    // it's on a different floor...
                    return new TextMessagePacket(MessageType.StatusSmall, "There is no way.").YieldSingleItem();
                }
            }

            return this.UseItemAt(player, itemUseInfo.ItemClientId, itemUseInfo.FromLocation, itemUseInfo.Index);
        }

        /// <summary>
        /// Attempts to use an item on behalf of a creature.
        /// </summary>
        /// <param name="creature">The creature using the item.</param>
        /// <param name="itemClientId">The id of the item attempting to be used.</param>
        /// <param name="fromLocation">The location from which the item is being used.</param>
        /// <param name="index">The index of the item being used.</param>
        private IEnumerable<IResponsePacket> UseItemAt(ICreature creature, ushort itemClientId, Location fromLocation, byte index)
        {
            var locationDiff = fromLocation - creature.Location;

            var useItemOperation =
                this.Context.OperationFactory.Create(
                    OperationType.UseItem,
                    new UseItemOperationCreationArguments(creature.Id, itemClientId, fromLocation, index));

            this.Context.EventRulesApi.ClearAllFor(useItemOperation.GetPartitionKey());
            this.Context.Scheduler.CancelAllFor(creature.Id, typeof(IMovementOperation));

            if (fromLocation.Type == LocationType.Map && locationDiff.MaxValueIn2D > 1)
            {
                // Too far away from it, we need to move closer first.
                var directions = this.Context.PathFinder.FindBetween(creature.Location, fromLocation, out Location retryLocation, onBehalfOfCreature: creature, considerAvoidsAsBlocking: true);

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

                    this.Context.EventRulesApi.SetupRule(new ExpediteOperationMovementEventRule(this.Logger, useItemOperation, conditionsForExpedition, 1), useItemOperation.GetPartitionKey());

                    this.ScheduleNewOperation(
                        this.Context.OperationFactory.Create(
                            OperationType.AutoWalk,
                            new AutoWalkOperationCreationArguments(creature.Id, creature, directions.ToArray())));
                }

                return null;
            }

            this.ScheduleNewOperation(useItemOperation);

            return null;
        }
    }
}