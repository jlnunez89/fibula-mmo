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
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Operations.Arguments;
    using Serilog;

    /// <summary>
    /// Class that represents a handler for a player moving something on the game.
    /// </summary>
    public class MoveThingHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveThingHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="operationFactory">A reference to the operation factory in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        public MoveThingHandler(ILogger logger, IOperationFactory operationFactory, IGameContext gameContext)
            : base(logger, operationFactory, gameContext)
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

            var responsePackets = new List<IOutgoingPacket>();

            if (!(this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return null;
            }

            player.ClearAllLocationActions();

            this.Context.Scheduler.CancelAllFor(player.Id, typeof(IMovementOperation));

            switch (moveThingInfo.FromLocation.Type)
            {
                case LocationType.Map:
                    this.HandleMoveFromMap(responsePackets, player, moveThingInfo);
                    break;
                case LocationType.InsideContainer:
                    this.HandleMoveFromContainer(player, moveThingInfo);
                    break;
                case LocationType.InventorySlot:
                    this.HandleMoveFromInventory(player, moveThingInfo);
                    break;
                default:
                    responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "You may not do this."));
                    break;
            }

            return responsePackets;
        }

        /// <summary>
        /// Handles the movement as a thing being moved from the map.
        /// </summary>
        /// <param name="responsePackets">A reference to the collection of response packets being sent.</param>
        /// <param name="player">The player involved in this request.</param>
        /// <param name="moveThingInfo">The information about the movement.</param>
        private void HandleMoveFromMap(List<IOutgoingPacket> responsePackets, IPlayer player, IThingMoveInfo moveThingInfo)
        {
            if (moveThingInfo.FromLocation.Type != LocationType.Map)
            {
                return;
            }

            // Before actually moving the item, check if we're close enough to use it.
            var locationDiff = moveThingInfo.FromLocation - player.Location;

            if (locationDiff.Z != 0)
            {
                // It's on a different floor...
                responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "There is no way."));

                return;
            }

            // First we turn towards the thing we're moving.
            if (player.Location != moveThingInfo.FromLocation)
            {
                var directionToThing = player.Location.DirectionTo(moveThingInfo.FromLocation);

                this.ScheduleNewOperation(OperationType.Turn, new TurnToDirectionOperationCreationArguments(player, directionToThing));
            }

            // Then we request the actual movement.
            this.MoveThingFromMap(player, moveThingInfo.ThingClientId, moveThingInfo.FromLocation, moveThingInfo.FromStackPos, moveThingInfo.ToLocation, moveThingInfo.Count);
        }

        /// <summary>
        /// Attempts to move a thing from the map on behalf of a player.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="thingId">The id of the thing attempting to be moved.</param>
        /// <param name="fromLocation">The location from which the thing is being moved.</param>
        /// <param name="fromStackPos">The position in the stack of the location from which the thing is being moved.</param>
        /// <param name="toLocation">The location to which the thing is being moved.</param>
        /// <param name="count">The amount of the thing that is being moved.</param>
        private void MoveThingFromMap(IPlayer player, ushort thingId, Location fromLocation, byte fromStackPos, Location toLocation, byte count)
        {
            player.ThrowIfNull(nameof(player));

            if (fromLocation.Type != LocationType.Map)
            {
                return;
            }

            var locationDiff = fromLocation - player.Location;

            if (locationDiff.MaxValueIn2D > 1)
            {
                // Too far away to move it, we need to move closer first.
                var directions = this.Context.PathFinder.FindBetween(player.Location, fromLocation, out Location retryLoc, onBehalfOfCreature: player, considerAvoidsAsBlock: true);

                if (directions == null || !directions.Any())
                {
                    return;
                }
                else
                {
                    // We basically add this request as the retry action, so that the request gets repeated when the player hits this location.
                    player.EnqueueRetryActionAtLocation(retryLoc, () => this.MoveThingFromMap(player, thingId, fromLocation, fromStackPos, toLocation, count));

                    this.AutoWalk(player, directions.ToArray());

                    return;
                }
            }

            if (!this.Context.TileAccessor.GetTileAt(fromLocation, out ITile sourceTile))
            {
                return;
            }

            var thingAtStackPos = sourceTile.GetTopThingByOrder(this.Context.CreatureFinder, fromStackPos);

            if (thingAtStackPos == null || thingAtStackPos.ThingId != thingId)
            {
                return;
            }

            // We know the source location is good, now let's figure out the destination to create the appropriate movement event.
            switch (toLocation.Type)
            {
                case LocationType.Map:
                    this.ScheduleNewOperation(
                        OperationType.MapToMapMovement,
                        new MapToMapMovementOperationCreationArguments(
                            player.Id,
                            thingAtStackPos,
                            fromLocation,
                            toLocation,
                            fromStackPos,
                            count),
                        thingAtStackPos is ICreature ? TimeSpan.FromSeconds(1) : TimeSpan.Zero);

                    break;
                case LocationType.InsideContainer:
                    this.ScheduleNewOperation(
                        OperationType.MapToContainerMovement,
                        new MapToContainerMovementOperationCreationArguments(
                            player.Id,
                            thingAtStackPos,
                            fromLocation,
                            player,
                            toLocation.ContainerId,
                            toLocation.ContainerIndex,
                            count));
                    break;
                case LocationType.InventorySlot:
                    this.ScheduleNewOperation(
                        OperationType.MapToBodyMovement,
                        new MapToBodyMovementOperationCreationArguments(
                            player.Id,
                            thingAtStackPos,
                            fromLocation,
                            player,
                            toLocation.Slot,
                            count));
                    break;
            }
        }

        /// <summary>
        /// Handles the movement as a thing being moved from a container.
        /// </summary>
        /// <param name="player">The player involved in this request.</param>
        /// <param name="moveThingInfo">The information about the movement.</param>
        private void HandleMoveFromContainer(IPlayer player, IThingMoveInfo moveThingInfo)
        {
            if (moveThingInfo.FromLocation.Type != LocationType.InsideContainer)
            {
                return;
            }

            this.MoveThingFromContainer(player, moveThingInfo.ThingClientId, moveThingInfo.FromLocation.ContainerId, moveThingInfo.FromLocation.ContainerIndex, moveThingInfo.ToLocation, moveThingInfo.Count);
        }

        /// <summary>
        /// Attempts to move a thing from a specific container on behalf of a player.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="thingId">The id of the thing attempting to be moved.</param>
        /// <param name="containerId">The id of the container from which the thing is being moved.</param>
        /// <param name="containerIndex">The index within the container from which the thing is being moved.</param>
        /// <param name="toLocation">The location to which the thing is being moved.</param>
        /// <param name="amount">The amount of the thing that is being moved.</param>
        private void MoveThingFromContainer(IPlayer player, ushort thingId, byte containerId, byte containerIndex, Location toLocation, byte amount)
        {
            player.ThrowIfNull(nameof(player));

            var sourceContainer = player.GetContainerById(containerId);

            var thingMoving = sourceContainer?[containerIndex];

            if (thingMoving == null || thingMoving.ThingId != thingId)
            {
                return;
            }

            // We know the source location is good, now let's figure out the destination to create the appropriate movement event.
            switch (toLocation.Type)
            {
                case LocationType.Map:
                    this.ScheduleNewOperation(
                        OperationType.ContainerToMapMovement,
                        new ContainerToMapMovementOperationCreationArguments(
                            player.Id,
                            thingMoving,
                            player,
                            containerId,
                            containerIndex,
                            toLocation,
                            amount));

                    break;
                case LocationType.InsideContainer:
                    this.ScheduleNewOperation(
                        OperationType.ContainerToContainerMovement,
                        new ContainerToContainerMovementOperationCreationArguments(
                            player.Id,
                            thingMoving,
                            player,
                            containerId,
                            containerIndex,
                            toLocation.ContainerId,
                            toLocation.ContainerIndex,
                            amount));

                    break;
                case LocationType.InventorySlot:
                    this.ScheduleNewOperation(
                        OperationType.ContainerToBodyMovement,
                        new ContainerToBodyMovementOperationCreationArguments(
                            player.Id,
                            thingMoving,
                            player,
                            containerId,
                            containerIndex,
                            toLocation.Slot,
                            amount));
                    break;
            }
        }

        /// <summary>
        /// Handles the movement as a thing being moved from an inventory slot.
        /// </summary>
        /// <param name="player">The player involved in this request.</param>
        /// <param name="moveThingInfo">The information about the movement.</param>
        private void HandleMoveFromInventory(IPlayer player, IThingMoveInfo moveThingInfo)
        {
            if (moveThingInfo.FromLocation.Type != LocationType.InventorySlot)
            {
                return;
            }

            this.MoveThingFromInventory(player, moveThingInfo.ThingClientId, moveThingInfo.FromLocation.Slot, moveThingInfo.ToLocation, moveThingInfo.Count);
        }

        /// <summary>
        /// Attempts to move a thing from an inventory slot on behalf of a player.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="thingId">The id of the thing attempting to be moved.</param>
        /// <param name="slot">The inventory slot from which the thing is being moved.</param>
        /// <param name="toLocation">The location to which the thing is being moved.</param>
        /// <param name="amount">The amount of the thing that is being moved.</param>
        private void MoveThingFromInventory(IPlayer player, ushort thingId, Slot slot, Location toLocation, byte amount)
        {
            player.ThrowIfNull(nameof(player));

            var sourceContainer = player.Inventory[(byte)slot] as IContainerItem;

            var thingMoving = sourceContainer?.Content.FirstOrDefault();

            if (thingMoving == null || thingMoving.ThingId != thingId)
            {
                return;
            }

            // We know the source location is good, now let's figure out the destination to create the appropriate movement event.
            switch (toLocation.Type)
            {
                case LocationType.Map:
                    this.ScheduleNewOperation(
                        OperationType.BodyToMapMovement,
                        new BodyToMapMovementOperationCreationArguments(
                            player.Id,
                            thingMoving,
                            player,
                            slot,
                            toLocation,
                            amount: amount));

                    break;
                case LocationType.InsideContainer:
                    this.ScheduleNewOperation(
                        OperationType.BodyToContainerMovement,
                        new BodyToContainerMovementOperationCreationArguments(
                            player.Id,
                            thingMoving,
                            player,
                            slot,
                            toLocation.ContainerId,
                            toLocation.ContainerIndex,
                            amount));

                    break;
                case LocationType.InventorySlot:
                    this.ScheduleNewOperation(
                        OperationType.BodyToBodyMovement,
                        new BodyToBodyMovementOperationCreationArguments(
                            player.Id,
                            thingMoving,
                            player,
                            slot,
                            toLocation.Slot,
                            amount));

                    break;
            }
        }
    }
}