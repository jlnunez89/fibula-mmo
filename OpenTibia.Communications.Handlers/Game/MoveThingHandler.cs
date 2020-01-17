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
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a handler for a player moving something on the game.
    /// </summary>
    public class MoveThingHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveThingHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        public MoveThingHandler(IGame gameInstance, ICreatureFinder creatureFinder)
            : base(gameInstance)
        {
            this.CreatureFinder = creatureFinder;
        }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.ItemThrow;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A value tuple with a value indicating whether the handler intends to respond, and a collection of <see cref="IOutgoingPacket"/>s that compose that response.</returns>
        public override (bool IntendsToRespond, IEnumerable<IOutgoingPacket> ResponsePackets) HandleRequest(INetworkMessage message, IConnection connection)
        {
            var moveThingInfo = message.ReadMoveThingInfo();

            var responsePackets = new List<IOutgoingPacket>();

            if (!(this.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return (false, null);
            }

            // A new request overrides and cancels any "auto" actions waiting to be retried.
            if (this.Game.PlayerRequest_CancelPendingMovements(player))
            {
                player.ClearAllLocationActions();
            }

            switch (moveThingInfo.FromLocation.Type)
            {
                case LocationType.Map:
                    this.HandleMoveFromMap(responsePackets, player, moveThingInfo);
                    break;
                case LocationType.InsideContainer:
                    this.HandleMoveFromContainer(responsePackets, player, moveThingInfo);
                    break;
                case LocationType.InventorySlot:
                    this.HandleMoveFromInventory(responsePackets, player, moveThingInfo);
                    break;
                default:
                    responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "You may not do this."));
                    break;
            }

            return (responsePackets.Any(), responsePackets);
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
            }

            // First we turn towards the thing we're moving.
            if (player.Location != moveThingInfo.FromLocation)
            {
                var directionToThing = player.Location.DirectionTo(moveThingInfo.FromLocation);

                this.Game.PlayerRequest_TurnToDirection(player, directionToThing);
            }

            // Then we request the actual movement.
            if (!responsePackets.Any() && !this.Game.PlayerRequest_MoveThingFromMap(player, moveThingInfo.ThingClientId, moveThingInfo.FromLocation, moveThingInfo.FromStackPos, moveThingInfo.ToLocation, moveThingInfo.Count))
            {
                responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "Sorry, not possible."));
            }
        }

        /// <summary>
        /// Handles the movement as a thing being moved from a container.
        /// </summary>
        /// <param name="responsePackets">A reference to the collection of response packets being sent.</param>
        /// <param name="player">The player involved in this request.</param>
        /// <param name="moveThingInfo">The information about the movement.</param>
        private void HandleMoveFromContainer(List<IOutgoingPacket> responsePackets, IPlayer player, IThingMoveInfo moveThingInfo)
        {
            if (moveThingInfo.FromLocation.Type != LocationType.InsideContainer)
            {
                return;
            }

            if (!responsePackets.Any() && !this.Game.PlayerRequest_MoveThingFromContainer(player, moveThingInfo.ThingClientId, moveThingInfo.FromLocation.ContainerId, moveThingInfo.FromLocation.ContainerIndex, moveThingInfo.ToLocation, moveThingInfo.Count))
            {
                responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "Sorry, not possible."));
            }
        }

        /// <summary>
        /// Handles the movement as a thing being moved from an inventory slot.
        /// </summary>
        /// <param name="responsePackets">A reference to the collection of response packets being sent.</param>
        /// <param name="player">The player involved in this request.</param>
        /// <param name="moveThingInfo">The information about the movement.</param>
        private void HandleMoveFromInventory(List<IOutgoingPacket> responsePackets, IPlayer player, IThingMoveInfo moveThingInfo)
        {
            if (moveThingInfo.FromLocation.Type != LocationType.InventorySlot)
            {
                return;
            }

            if (!responsePackets.Any() && !this.Game.PlayerRequest_MoveThingFromInventory(player, moveThingInfo.ThingClientId, moveThingInfo.FromLocation.Slot, moveThingInfo.ToLocation, moveThingInfo.Count))
            {
                responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "Sorry, not possible."));
            }
        }
    }
}