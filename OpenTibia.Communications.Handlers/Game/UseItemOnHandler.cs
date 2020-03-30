// -----------------------------------------------------------------
// <copyright file="UseItemOnHandler.cs" company="2Dudes">
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
    /// Class that represents an "item used on something" handler for the game server.
    /// </summary>
    public class UseItemOnHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemOnHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        public UseItemOnHandler(ILogger logger, IGameContext gameContext)
            : base(logger, gameContext)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.ItemUseOn;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutgoingPacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            var itemUseOnInfo = message.ReadItemUseOnInfo();

            if (!(this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return null;
            }

            //player.ClearAllLocationBasedOperations();

            this.Context.Scheduler.CancelAllFor(player.Id, typeof(IMovementOperation));

            // Before actually using the item, check if we're close enough to use it.
            if (itemUseOnInfo.FromLocation.Type == LocationType.Map)
            {
                // Turn to the item if it's not exactly the location.
                if (player.Location != itemUseOnInfo.FromLocation)
                {
                    var directionToThing = player.Location.DirectionTo(itemUseOnInfo.FromLocation);

                    this.ScheduleNewOperation(
                        this.Context.OperationFactory.Create(
                            OperationType.Turn,
                            new TurnToDirectionOperationCreationArguments(player, directionToThing)));
                }

                var locationDiff = itemUseOnInfo.FromLocation - player.Location;

                if (locationDiff.Z != 0)
                {
                    // it's on a different floor...
                    return new TextMessagePacket(MessageType.StatusSmall, "There is no way.").YieldSingleItem();
                }
            }

            return this.UseItemAtOn(player, itemUseOnInfo.FromItemClientId, itemUseOnInfo.FromLocation, itemUseOnInfo.FromIndex, itemUseOnInfo.ToItemClientId, itemUseOnInfo.ToLocation, itemUseOnInfo.ToIndex);
        }

        /// <summary>
        /// Attempts to use an item on something else, on behalf of a creature.
        /// </summary>
        /// <param name="creature">The creature using the item.</param>
        /// <param name="fromItemClientId">The id of the item attempting to be used.</param>
        /// <param name="fromLocation">The location from which the item is being used.</param>
        /// <param name="fromIndex">The index of the item being used.</param>
        /// <param name="toThingId">The id of the thing on which the item is being used on.</param>
        /// <param name="toLocation">The location the thing on which the item is being used on.</param>
        /// <param name="toIndex">The index of the thing on which the item is being used on.</param>
        private IEnumerable<IOutgoingPacket> UseItemAtOn(ICreature creature, ushort fromItemClientId, Location fromLocation, byte fromIndex, ushort toThingId, Location toLocation, byte toIndex)
        {
            var locationDiff = fromLocation - creature.Location;

            var useItemOnOperation =
                this.Context.OperationFactory.Create(
                    OperationType.UseItemOn,
                    new UseItemOnOperationCreationArguments(creature.Id, fromItemClientId, fromLocation, fromIndex, toThingId, toLocation, toIndex));

            if (fromLocation.Type == LocationType.Map && locationDiff.MaxValueIn2D > 1)
            {
                // Too far away from it, we need to move closer first.
                var directions = this.Context.PathFinder.FindBetween(creature.Location, fromLocation, out Location retryLocation, onBehalfOfCreature: creature, considerAvoidsAsBlock: true);

                if (directions == null || !directions.Any())
                {
                    return new TextMessagePacket(MessageType.StatusSmall, OperationMessage.ThereIsNoWay).YieldSingleItem();
                }
                else
                {
                    //creature.SetOperationAtLocation(retryLocation, useItemOnOperation);

                    this.ScheduleNewOperation(
                        this.Context.OperationFactory.Create(
                            OperationType.AutoWalk,
                            new AutoWalkOperationCreationArguments(creature.Id, creature, directions.ToArray())));
                }

                return null;
            }

            this.ScheduleNewOperation(useItemOnOperation);

            return null;
        }
    }
}