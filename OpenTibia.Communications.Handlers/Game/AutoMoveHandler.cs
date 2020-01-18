// -----------------------------------------------------------------
// <copyright file="AutoMoveHandler.cs" company="2Dudes">
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
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a handler for auto movement requests.
    /// </summary>
    public class AutoMoveHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMoveHandler"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public AutoMoveHandler(
            ICreatureFinder creatureFinder,
            IGame gameInstance)
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
        public override byte ForPacketType => (byte)IncomingGamePacketType.AutoMove;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A value tuple with a value indicating whether the handler intends to respond, and a collection of <see cref="IOutgoingPacket"/>s that compose that response.</returns>
        public override (bool IntendsToRespond, IEnumerable<IOutgoingPacket> ResponsePackets) HandleRequest(INetworkMessage message, IConnection connection)
        {
            var automovementInfo = message.ReadAutomovementInfo();

            if (!(this.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return (false, null);
            }

            var responsePackets = new List<IOutgoingPacket>();

            // A new request overrides and cancels any "auto" actions waiting to be retried.
            if (this.Game.PlayerRequest_CancelPendingMovements(player))
            {
                player.ClearAllLocationActions();
            }

            if (!this.Game.PlayerRequest_AutoWalk(player, automovementInfo.Directions))
            {
                responsePackets.Add(new PlayerWalkCancelPacket(automovementInfo.Directions[0]));

                responsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "Sorry, not possible."));
            }

            return (responsePackets.Count > 0, responsePackets);
        }
    }
}