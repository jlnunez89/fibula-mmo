// -----------------------------------------------------------------
// <copyright file="RotateItemHandler.cs" company="2Dudes">
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
    /// Class that represents an item rotation handler for the game server.
    /// </summary>
    public class RotateItemHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateItemHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="operationFactory">A reference to the operation factory in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        public RotateItemHandler(ILogger logger, IOperationFactory operationFactory, IGameContext gameContext)
            : base(logger, operationFactory, gameContext)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.ItemRotate;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutgoingPacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            var itemRotationInfo = message.ReadItemRotationInfo();

            if (!(this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return null;
            }

            player.ClearAllLocationActions();

            this.Context.Scheduler.CancelAllFor(player.Id, typeof(IMovementOperation));

            // Before actually using the item, check if we're close enough to use it.
            if (itemRotationInfo.AtLocation.Type == LocationType.Map)
            {
                // Turn to the item if it's not exactly the location.
                if (player.Location != itemRotationInfo.AtLocation)
                {
                    var directionToThing = player.Location.DirectionTo(itemRotationInfo.AtLocation);

                    this.ScheduleNewOperation(OperationType.Turn, new TurnToDirectionOperationCreationArguments(player, directionToThing));
                }

                var locationDiff = itemRotationInfo.AtLocation - player.Location;

                if (locationDiff.Z != 0)
                {
                    // it's on a different floor...
                    return new TextMessagePacket(MessageType.StatusSmall, "There is no way.").YieldSingleItem();
                }
            }

            this.RotateItemAt(player, itemRotationInfo.AtLocation, itemRotationInfo.Index, itemRotationInfo.ItemClientId);

            return null;
        }

        /// <summary>
        /// Attempts to rotate an item.
        /// </summary>
        /// <param name="creature">The creature requesting the rotation.</param>
        /// <param name="atLocation">The location at which the item to rotate is.</param>
        /// <param name="index">The index where the item to rotate is.</param>
        /// <param name="typeId">The type id of the item to rotate.</param>
        private void RotateItemAt(ICreature creature, Location atLocation, byte index, ushort typeId)
        {
            var locationDiff = atLocation - creature.Location;

            if (atLocation.Type == LocationType.Map && locationDiff.MaxValueIn2D > 1)
            {
                // Too far away to move it, we need to move closer first.
                var directions = this.Context.PathFinder.FindBetween(creature.Location, atLocation, out Location retryLoc, onBehalfOfCreature: creature, considerAvoidsAsBlock: true);

                if (directions == null || !directions.Any())
                {
                    return;
                }
                else
                {
                    // We basically add this request as the retry action, so that the request gets repeated when the player hits this location.
                    creature.EnqueueRetryActionAtLocation(retryLoc, () => this.RotateItemAt(creature, atLocation, index, typeId));

                    this.AutoWalk(creature, directions.ToArray());

                    return;
                }
            }

            // TODO: should this be scheduled too?
            if (this.Context.TileAccessor.GetTileAt(atLocation, out ITile targetTile))
            {
                if (!(targetTile.GetTopThingByOrder(this.Context.CreatureFinder, index) is IItem item) || item.ThingId != typeId || !item.CanBeRotated)
                {
                    return;
                }

                this.ScheduleNewOperation(
                        OperationType.ChangeItem,
                        new ChangeItemOperationCreationArguments(
                            requestorId: 0,
                            item.ThingId,
                            atLocation,
                            item.RotateTo));
            }
        }
    }
}