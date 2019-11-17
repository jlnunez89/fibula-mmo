// <copyright file="PlayerSetModeHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers.Game
{
    using System;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    public class PlayerSetModeHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSetModeHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public PlayerSetModeHandler(IGame gameInstance)
            : base(gameInstance)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.ChangeModes;

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

            var rawFightMode = message.GetByte(); // 1 - offensive, 2 - balanced, 3 - defensive
            var rawChaseMode = message.GetByte(); // 0 - stand while fightning, 1 - chase opponent
            var rawSafeMode = message.GetByte();

            var fightMode = (FightMode)rawFightMode;

            // TODO: correctly implement.
            Console.WriteLine($"PlayerId {player.Name} changed modes to {fightMode}.");
        }
    }
}