// <copyright file="AttackHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers.Game
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a handler for attack requests.
    /// </summary>
    public class AttackHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public AttackHandler(IGame gameInstance)
            : base(gameInstance)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.Attack;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            var attackInfo = message.ReadAttackInfo();

            var player = this.Game.GetCreatureWithId(connection.PlayerId) as ICombatant;

            player?.SetAttackTarget(attackInfo.TargetCreatureId);
        }
    }
}