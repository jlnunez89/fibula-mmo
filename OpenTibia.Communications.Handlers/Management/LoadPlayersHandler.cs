// <copyright file="LoadPlayersHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers.Management
{
    using System;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Outgoing;

    public class LoadPlayersHandler : BaseHandler
    {
        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingManagementPacketType.LoadPlayers;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            var loadPlayersPacket = message.ReadDefaultInfo();

            using (var otContext = new OpenTibiaDbContext())
            {
                var thirtyDaysBack = new DateTimeOffset(DateTime.Today).AddDays(-30).ToUnixTimeSeconds();

                var loadedPlayers = otContext.Players.Where(p => p.Lastlogin > thirtyDaysBack);

                this.ResponsePackets.Add(new LoadPlayersResultPacket(loadedPlayers.ToList()));
            }
        }
    }
}