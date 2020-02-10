// -----------------------------------------------------------------
// <copyright file="UseItemHandler.cs" company="2Dudes">
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
    using OpenTibia.Server.Operations.Arguments;
    using Serilog;

    /// <summary>
    /// Class that represents an item use handler for the game server.
    /// </summary>
    public class UseItemHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="operationFactory">A reference to the operation factory in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        public UseItemHandler(ILogger logger, IOperationFactory operationFactory, IGameContext gameContext)
            : base(logger, operationFactory, gameContext)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.ItemUse;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutgoingPacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            var itemUseInfo = message.ReadItemUseInfo();

            if (!(this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return null;
            }

            player.ClearAllLocationActions();

            this.Context.Scheduler.CancelAllFor(player.Id, typeof(IMovementOperation));

            // Before actually using the item, check if we're close enough to use it.
            if (itemUseInfo.FromLocation.Type == LocationType.Map)
            {
                // Turn to the item if it's not exactly the location.
                if (player.Location != itemUseInfo.FromLocation)
                {
                    var directionToThing = player.Location.DirectionTo(itemUseInfo.FromLocation);

                    this.ScheduleNewOperation(OperationType.Turn, new TurnToDirectionOperationCreationArguments(player, directionToThing));
                }

                var locationDiff = itemUseInfo.FromLocation - player.Location;

                if (locationDiff.Z != 0)
                {
                    // it's on a different floor...
                    return new TextMessagePacket(MessageType.StatusSmall, "There is no way.").YieldSingleItem();
                }
            }

            this.UseItemAt(player, itemUseInfo.ItemClientId, itemUseInfo.FromLocation, itemUseInfo.FromStackPos, itemUseInfo.Index);

            return null;
        }

        /// <summary>
        /// Attempts to use an item on behalf of a creature.
        /// </summary>
        /// <param name="creature">The creature using the item.</param>
        /// <param name="itemClientId">The id of the item attempting to be used.</param>
        /// <param name="fromLocation">The location from which the item is being used.</param>
        /// <param name="fromStackPos">The position in the stack of the location from which the item is being used.</param>
        /// <param name="index">The index of the item being used.</param>
        private void UseItemAt(ICreature creature, ushort itemClientId, Location fromLocation, byte fromStackPos, byte index)
        {
            var locationDiff = fromLocation - creature.Location;

            if (fromLocation.Type == LocationType.Map && locationDiff.MaxValueIn2D > 1)
            {
                // Too far away to move it, we need to move closer first.
                var directions = this.Context.PathFinder.FindBetween(creature.Location, fromLocation, out Location retryLoc, onBehalfOfCreature: creature, considerAvoidsAsBlock: true);

                if (directions == null || !directions.Any())
                {
                    return;
                }
                else
                {
                    // We basically add this request as the retry action, so that the request gets repeated when the player hits this location.
                    creature.EnqueueRetryActionAtLocation(retryLoc, () => this.UseItemAt(creature, itemClientId, fromLocation, fromStackPos, index));

                    this.AutoWalk(creature, directions.ToArray());

                    return;
                }
            }

            this.ScheduleNewOperation(
                OperationType.UseItem,
                new UseItemOperationCreationArguments(
                    creature.Id,
                    itemClientId,
                    fromLocation,
                    fromStackPos,
                    index));
        }
    }
}