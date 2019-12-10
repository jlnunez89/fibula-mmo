// -----------------------------------------------------------------
// <copyright file="PlayerWalkOnDemandHandler.cs" company="2Dudes">
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
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Abstract class that represents the base for all player walk handlers.
    /// </summary>
    public abstract class PlayerWalkOnDemandHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerWalkOnDemandHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="direction">The direction to walk to.</param>
        public PlayerWalkOnDemandHandler(IGame gameInstance, ICreatureFinder creatureFinder, Direction direction)
            : base(gameInstance)
        {
            this.CreatureFinder = creatureFinder;
            this.Direction = direction;
        }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the direction to walk to.
        /// </summary>
        public Direction Direction { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A value tuple with a value indicating whether the handler intends to respond, and a collection of <see cref="IOutgoingPacket"/>s that compose that response.</returns>
        public override (bool IntendsToRespond, IEnumerable<IOutgoingPacket> ResponsePackets) HandleRequest(INetworkMessage message, IConnection connection)
        {
            if (!(this.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return (false, null);
            }

            var responsePackets = new List<IOutgoingPacket>();

            if (!this.Game.PlayerRequest_WalkToDirection(player, this.Direction))
            {
                responsePackets.Add(new PlayerWalkCancelPacket(this.Direction));

                responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "Sorry, not possible."));
            }

            return (responsePackets.Count > 0, responsePackets);
        }
    }
}