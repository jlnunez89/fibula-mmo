// -----------------------------------------------------------------
// <copyright file="SetModeHandler.cs" company="2Dudes">
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
    using System;
    using System.Collections.Generic;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a set mode handler for the game server.
    /// </summary>
    public class SetModeHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetModeHandler"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public SetModeHandler(ICreatureFinder creatureFinder, IGame gameInstance)
            : base(gameInstance)
        {
            this.CreatureFinder = creatureFinder;
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.ChangeModes;

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
            var fightModesInfo = message.ReadModesInfo();

            if (!(this.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return (false, null);
            }

            // TODO: correctly implement.
            Console.WriteLine($"{player.Name} changed modes to {fightModesInfo.FightMode} and {fightModesInfo.ChaseMode}, safety: {fightModesInfo.SafeModeOn}.");

            // player.SetFightMode(fightModesInfo.FightMode);
            // player.SetChaseMode(fightModesInfo.ChaseMode);
            // player.SetSafetyMode(fightModesInfo.ChaseMode);

            return (false, null);
        }
    }
}