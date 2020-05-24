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

namespace OpenTibia.Communications.Protocol772.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Communications.Packets;
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
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        public LookAtHandler(ILogger logger, IGameContext gameContext)
            : base(logger, gameContext)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.LookAt;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutgoingPacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            var lookAtInfo = message.ReadLookAtInfo();

            IThing thing = null;

            var responsePackets = new List<IOutgoingPacket>();

            if (!(this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return null;
            }

            // TODO: make async, this does not need to be synchronous.
            if (lookAtInfo.Location.Type != LocationType.Map || player.CanSee(lookAtInfo.Location))
            {
                IContainerItem container;

                // Get thing at location
                switch (lookAtInfo.Location.Type)
                {
                    case LocationType.Map:
                        thing = this.Context.TileAccessor.GetTileAt(lookAtInfo.Location, out ITile targetTile) ? targetTile.GetTopThingByOrder(this.Context.CreatureFinder, lookAtInfo.StackPosition) : null;
                        break;
                    case LocationType.InsideContainer:
                        container = this.Context.ContainerManager.FindForCreature(player.Id, lookAtInfo.Location.ContainerId);

                        thing = container?[lookAtInfo.Location.ContainerIndex];
                        break;
                    case LocationType.InventorySlot:
                        container = player.Inventory[(byte)lookAtInfo.Location.Slot] as IContainerItem;

                        thing = container?.Content.FirstOrDefault();
                        break;
                }

                if (thing != null)
                {
                    this.Logger.Debug($"Player {player.Name} looking at {thing}. {lookAtInfo.Location} sector: {lookAtInfo.Location.X / 32}-{lookAtInfo.Location.Y / 32}-{lookAtInfo.Location.Z:00}");

                    responsePackets.Add(new TextMessagePacket(MessageType.DescriptionGreen, $"You see {thing.GetDescription(player)}"));
                }
            }

            return responsePackets;
        }
    }
}