// -----------------------------------------------------------------
// <copyright file="ActionWithoutContentHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Mechanics.Handlers
{
    using System.Collections.Generic;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Communications.Packets.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Server.Mechanics.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents a request handler for actions with no content to be read, for the game server.
    /// </summary>
    public class ActionWithoutContentHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionWithoutContentHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameInstance">A reference to the game instance.</param>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        public ActionWithoutContentHandler(ILogger logger, IGame gameInstance, ICreatureFinder creatureFinder)
            : base(logger, gameInstance)
        {
            this.CreatureFinder = creatureFinder;
        }

        /// <summary>
        /// Gets the creature finder to use.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="incomingPacket">The packet to handle.</param>
        /// <param name="client">A reference to the client from where this request originated from, for context.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutboundPacket> HandleRequest(IIncomingPacket incomingPacket, IClient client)
        {
            incomingPacket.ThrowIfNull(nameof(incomingPacket));
            client.ThrowIfNull(nameof(client));

            if (!(incomingPacket is IActionWithoutContentInfo actionInfo))
            {
                this.Logger.Error($"Expected packet info of type {nameof(IActionWithoutContentInfo)} but got {incomingPacket.GetType().Name}.");

                return null;
            }

            if (!(this.CreatureFinder.FindCreatureById(client.PlayerId) is IPlayer player))
            {
                this.Logger.Warning($"Client's associated player could not be found. [Id={client.PlayerId}]");

                return null;
            }

            // player.ClearAllLocationBasedOperations();
            // this.Context.Scheduler.CancelAllFor(player.Id, typeof(IMovementOperation));

            switch (actionInfo.Action)
            {
                case IncomingGamePacketType.LogOut:
                    this.Game.LogPlayerOut(player);
                    break;
            }

            return null;
        }
    }
}