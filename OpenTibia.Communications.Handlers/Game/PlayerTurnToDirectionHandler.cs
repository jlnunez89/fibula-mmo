// <copyright file="PlayerTurnToDirectionHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers.Game
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    public abstract class PlayerTurnToDirectionHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerTurnToDirectionHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        /// <param name="direction"></param>
        public PlayerTurnToDirectionHandler(IGame gameInstance, Direction direction)
            : base(gameInstance)
        {
            this.Direction = direction;
        }

        public Direction Direction { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            // No other content in message.
            if (!(this.Game.GetCreatureWithId(connection.PlayerId) is IPlayer player))
            {
                return;
            }

            player.TurnToDirection(this.Direction);

            this.Game.NotifySpectatingPlayers(conn => new CreatureTurnedNotification(conn, player), player.Location);
        }
    }
}