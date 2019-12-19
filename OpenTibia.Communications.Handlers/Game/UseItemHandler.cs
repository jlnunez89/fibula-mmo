// -----------------------------------------------------------------
// <copyright file="ItemUseHandler.cs" company="2Dudes">
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
    using Serilog;

    /// <summary>
    /// Class that represents an item use handler for the game server.
    /// </summary>
    public class UseItemHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger to use in this handler.</param>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public UseItemHandler(
            ILogger logger,
            ICreatureFinder creatureFinder,
            IGame gameInstance)
            : base(gameInstance)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            this.Logger = logger.ForContext<UseItemHandler>();
            this.CreatureFinder = creatureFinder;
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.ItemUse;

        /// <summary>
        /// Gets the logger to use in this handler.
        /// </summary>
        public ILogger Logger { get; }

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
            var itemUseInfo = message.ReadItemUseInfo();

            var responsePackets = new List<IOutgoingPacket>();

            if (!(this.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return (false, null);
            }

            // player.ClearPendingActions();

            // Before actually using the item, check if we're close enough to use it.
            if (itemUseInfo.FromLocation.Type == LocationType.Ground)
            {
                var locationDiff = itemUseInfo.FromLocation - player.Location;

                if (locationDiff.Z != 0)
                {
                    // it's on a different floor...
                    responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "There is no way."));
                }
                else if (locationDiff.MaxValueIn2D > 1)
                {
                    // Too far away to move it.
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

            if (player.Location != itemUseInfo.FromLocation)
            {
                var directionToThing = player.Location.DirectionTo(itemUseInfo.FromLocation);

                this.Game.PlayerRequest_TurnToDirection(player, directionToThing);
            }

            if (!responsePackets.Any() && !this.Game.PlayerRequest_UseItem(player, itemUseInfo.ItemClientId, itemUseInfo.FromLocation, itemUseInfo.FromStackPos, itemUseInfo.Index))
            {
                responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "Sorry, not possible."));
            }

            return (responsePackets.Any(), responsePackets);
        }
    }
}