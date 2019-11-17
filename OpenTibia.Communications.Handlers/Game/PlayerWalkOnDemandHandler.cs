// <copyright file="PlayerWalkOnDemandHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers.Game
{
    using System;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    public abstract class PlayerWalkOnDemandHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerWalkOnDemandHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        /// <param name="direction">The direction to walk to.</param>
        public PlayerWalkOnDemandHandler(IGame gameInstance, Direction direction)
            : base(gameInstance)
        {
            this.Direction = direction;
        }

        /// <summary>
        /// Gets the direction to walk to.
        /// </summary>
        public Direction Direction { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            if (!(this.Game.GetCreatureWithId(connection.PlayerId) is IPlayer player))
            {
                return;
            }

            player.ClearPendingActions();

            var cooldownRemaining = player.CalculateRemainingCooldownTime(ExhaustionType.Move, DateTimeOffset.UtcNow);

            if (!this.Game.RequestCreatureWalkToDirection(player, this.Direction, cooldownRemaining))
            {
                this.ResponsePackets.Add(new PlayerWalkCancelPacket(player.Direction));

                this.ResponsePackets.Add(new TextMessagePacket(MessageType.StatusSmall, "Sorry, not possible."));
            }
        }
    }
}