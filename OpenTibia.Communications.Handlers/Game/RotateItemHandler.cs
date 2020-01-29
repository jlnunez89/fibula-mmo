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
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents an item rotation handler for the game server.
    /// </summary>
    public class RotateItemHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateItemHandler"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public RotateItemHandler(ICreatureFinder creatureFinder, IGame gameInstance)
            : base(gameInstance)
        {
            this.CreatureFinder = creatureFinder;
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.ItemRotate;

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A value tuple with a value indicating whether the handler intends to respond, and a collection of <see cref="IOutgoingPacket"/>s that compose that response.</returns>
        public override (bool IntendsToRespond, IEnumerable<IOutgoingPacket> ResponsePackets) HandleRequest(INetworkMessage message, IConnection connection)
        {
            var itemRotationInfo = message.ReadItemRotationInfo();

            var responsePackets = new List<IOutgoingPacket>();

            if (!(this.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return (false, null);
            }

            player.ClearAllLocationActions();

            this.Game.Request_CancelMovements(player);

            // Before actually using the item, check if we're close enough to use it.
            if (itemRotationInfo.AtLocation.Type == LocationType.Map)
            {
                // Turn to the item if it's not exactly the location.
                if (player.Location != itemRotationInfo.AtLocation)
                {
                    var directionToThing = player.Location.DirectionTo(itemRotationInfo.AtLocation);

                    this.Game.Request_TurnToDirection(player, directionToThing);
                }

                var locationDiff = itemRotationInfo.AtLocation - player.Location;

                if (locationDiff.Z != 0)
                {
                    // it's on a different floor...
                    responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "There is no way."));
                }
            }

            if (!responsePackets.Any() && !this.Game.PlayerRequest_RotateItemAt(player, itemRotationInfo.AtLocation, itemRotationInfo.Index, itemRotationInfo.ItemClientId))
            {
                responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "Sorry, not possible."));
            }

            return (responsePackets.Any(), responsePackets);
        }
    }
}