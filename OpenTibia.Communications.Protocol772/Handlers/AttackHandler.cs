// -----------------------------------------------------------------
// <copyright file="AttackHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Protocol772.Handlers
{
    using System.Collections.Generic;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Server.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents a handler for attack requests.
    /// </summary>
    public class AttackHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        public AttackHandler(ILogger logger, IGameContext gameContext)
            : base(logger, gameContext)
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
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutgoingPacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            var attackInfo = message.ReadAttackInfo();

            if (this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player)
            {
                this.Context.Scheduler.CancelAllFor(player.Id, typeof(ICombatOperation));

                var combatantTarget = this.Context.CreatureFinder.FindCreatureById(attackInfo.TargetCreatureId) as ICombatant;

                // TODO: make this an operation as well.
                player.SetAttackTarget(combatantTarget);
            }

            return null;
        }
    }
}