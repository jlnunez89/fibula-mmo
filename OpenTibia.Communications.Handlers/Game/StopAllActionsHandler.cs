// -----------------------------------------------------------------
// <copyright file="StopAllActionsHandler.cs" company="2Dudes">
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
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a handler for a request to stop all actions.
    /// </summary>
    public class StopAllActionsHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StopAllActionsHandler"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public StopAllActionsHandler(
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
        public override byte ForPacketType => (byte)IncomingGamePacketType.StopAllActions;

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

            // player?.ClearPendingActions();

            if (this.Game.PlayerRequest_CancelPendingMovements(player))
            {
                responsePackets.Add(new PlayerWalkCancelPacket(player.Direction.GetClientSafeDirection()));
            }

            return (responsePackets.Count > 0, responsePackets);
        }
    }
}