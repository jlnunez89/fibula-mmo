// <copyright file="ItemMoveHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers.Game
{
    using System.Linq;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    public class ItemMoveHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemMoveHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public ItemMoveHandler(IGame gameInstance)
            : base(gameInstance)
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
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            var itemMoveInfo = message.ReadItemMoveInfo();

            if (!(this.Game.GetCreatureWithId(connection.PlayerId) is IPlayer player))
            {
                return;
            }

            player.ClearPendingActions();

            // Before actually moving the item, check if we're close enough to use it.
            if (itemMoveInfo.FromLocation.Type == LocationType.Ground)
            {
                var locationDiff = itemMoveInfo.FromLocation - player.Location;

                if (locationDiff.Z != 0) // it's on a different floor...
                {
                    this.ResponsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "There is no way."));

                    return;
                }

                if (locationDiff.MaxValueIn2D > 1)
                {
                    // Too far away to use it.
                    var directions = this.Game.Pathfind(player.Location, itemMoveInfo.FromLocation, out Location retryLoc).ToArray();

                    player.SetPendingAction(new MoveItemPlayerAction(player, itemMoveInfo, retryLoc));

                    if (directions.Length > 0)
                    {
                        player.AutoWalk(directions);
                    }
                    else // we found no way...
                    {
                        this.ResponsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "There is no way."));
                    }

                    return;
                }
            }

            new MoveItemPlayerAction(player, itemMoveInfo, itemMoveInfo.FromLocation).Perform();
        }
    }
}