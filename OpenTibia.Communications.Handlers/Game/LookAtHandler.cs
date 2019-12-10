// -----------------------------------------------------------------
// <copyright file="LookAtHandler.cs" company="2Dudes">
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
    using System.Linq;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents a look at handler for the game server.
    /// </summary>
    public class LookAtHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LookAtHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger to use in this handler.</param>
        /// <param name="gameInstance">A reference to the game instance.</param>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="tileAccessor">A reference to the tile accessor.</param>
        public LookAtHandler(
            ILogger logger,
            IGame gameInstance,
            ICreatureFinder creatureFinder,
            ITileAccessor tileAccessor)
            : base(gameInstance)
        {
            this.CreatureFinder = creatureFinder;
            this.TileAccessor = tileAccessor;
            this.Logger = logger.ForContext<PlayerLoginHandler>();
        }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the reference to the tile accessor.
        /// </summary>
        public ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Gets the logger to use in this handler.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.LookAt;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A value tuple with a value indicating whether the handler intends to respond, and a collection of <see cref="IOutgoingPacket"/>s that compose that response.</returns>
        public override (bool IntendsToRespond, IEnumerable<IOutgoingPacket> ResponsePackets) HandleRequest(INetworkMessage message, IConnection connection)
        {
            var lookAtInfo = message.ReadLookAtInfo();

            IThing thing = null;

            var responsePackets = new List<IOutgoingPacket>();

            if (!(this.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return (false, null);
            }

            this.Logger.Verbose($"Player {player.Name} looking at thing with id: {lookAtInfo.ThingId}. {lookAtInfo.Location}");

            if (lookAtInfo.Location.Type != LocationType.Ground || player.CanSee(lookAtInfo.Location))
            {
                // Get thing at location
                switch (lookAtInfo.Location.Type)
                {
                    case LocationType.Ground:
                        thing = this.TileAccessor.GetTileAt(lookAtInfo.Location, out ITile targetTile) ? targetTile.GetThingAtStackPosition(this.CreatureFinder, lookAtInfo.StackPosition) : null;
                        break;
                    case LocationType.Container:
                        // TODO: implement containers.
                        // Container container = player.Inventory.GetContainer(location.Container);
                        // if (container != null)
                        // {
                        //    return container.GetItem(location.ContainerPosition);
                        // }
                        break;
                    case LocationType.Slot:
                        thing = player.Inventory[(byte)lookAtInfo.Location.Slot];
                        break;
                }

                if (thing != null)
                {
                    responsePackets.Add(new TextMessagePacket(MessageType.DescriptionGreen, $"You see {thing.InspectionText}."));
                }
            }

            return (responsePackets.Any(), responsePackets);
        }
    }
}