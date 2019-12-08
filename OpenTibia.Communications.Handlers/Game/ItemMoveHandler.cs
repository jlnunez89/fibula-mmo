// -----------------------------------------------------------------
// <copyright file="ItemMoveHandler.cs" company="2Dudes">
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
    /// Class that represents an item movement handler for the game server.
    /// </summary>
    public class ItemMoveHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemMoveHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        public ItemMoveHandler(IGame gameInstance, ICreatureFinder creatureFinder)
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
            var itemMoveInfo = message.ReadItemMoveInfo();

            var responsePackets = new List<IOutgoingPacket>();

            if (!(this.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return (false, null);
            }

            // player.ClearPendingActions();

            // Before actually moving the item, check if we're close enough to use it.
            if (itemMoveInfo.FromLocation.Type == LocationType.Ground)
            {
                var locationDiff = itemMoveInfo.FromLocation - player.Location;

                if (locationDiff.Z != 0)
                {
                    // it's on a different floor...
                    responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "There is no way."));
                }
                else if (locationDiff.MaxValueIn2D > 1)
                {
                    // Too far away to use it.
                    //var directions = this.Game.Pathfind(player.Location, itemMoveInfo.FromLocation, out Location retryLoc).ToArray();

                    //player.SetPendingAction(new MoveItemPlayerAction(player, itemMoveInfo, retryLoc));

                    //if (directions.Length > 0)
                    //{
                    //    player.AutoWalk(directions);
                    //}
                    //else // we found no way...
                    //{
                    //    responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "There is no way."));
                    //}

                    responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "Too far away, auto walk is not implemented yet."));
                }
            }

            if (!responsePackets.Any() && !this.Game.PlayerMoveThing(player, itemMoveInfo.ClientId, itemMoveInfo.FromLocation, itemMoveInfo.FromStackPos, itemMoveInfo.ToLocation, itemMoveInfo.Count))
            {
                responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "Sorry, not possible."));
            }

            return (responsePackets.Any(), responsePackets);
        }
    }
}