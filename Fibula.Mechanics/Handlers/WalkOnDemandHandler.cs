// -----------------------------------------------------------------
// <copyright file="WalkOnDemandHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Handlers
{
    using System.Collections.Generic;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Constants;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Abstract class that represents the base for all player walk handlers.
    /// </summary>
    public class WalkOnDemandHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WalkOnDemandHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameInstance">A reference to the game instance.</param>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        public WalkOnDemandHandler(ILogger logger, IGame gameInstance, ICreatureFinder creatureFinder)
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

            if (!(incomingPacket is IWalkOnDemandInfo walkOnDemandInfo))
            {
                this.Logger.Error($"Expected packet info of type {nameof(IWalkOnDemandInfo)} but got {incomingPacket.GetType().Name}.");

                return null;
            }

            if (!(this.CreatureFinder.FindCreatureById(client.PlayerId) is IPlayer player))
            {
                this.Logger.Warning($"Client's associated player could not be found. [Id={client.PlayerId}]");

                return null;
            }

            // player.ClearAllLocationBasedOperations
            var nextLocation = player.Location.LocationAt(walkOnDemandInfo.Direction);

            // this.Game.CancelPlayerActions(player, typeof(MovementOperation));
            this.Game.Movement(player.Id, CreatureConstants.CreatureThingId, player.Location, fromIndex: byte.MaxValue, player.Id, nextLocation, player.Id);

            return null;
        }
    }
}