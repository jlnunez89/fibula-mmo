// <copyright file="CreatePlayerListHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers.Management
{
    using System.Collections.Generic;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Outgoing;

    public class CreatePlayerListHandler : BaseHandler
    {
        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingManagementPacketType.CreatePlayerList;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            var playerListInfo = message.ReadPlayerListInfo();

            using (var otContext = new OpenTibiaDbContext())
            {
                var currentRecord = otContext.Stats.Select(s => s.RecordOnline).FirstOrDefault();
                var isNewRecord = playerListInfo.PlayerList.Count > currentRecord;

                var currentRemove = new Dictionary<string, OnlinePlayer>();

                foreach (var player in otContext.Online.ToList())
                {
                    currentRemove.Add(player.Name, player);
                }

                foreach (var player in playerListInfo.PlayerList)
                {
                    var dbRecord = otContext.Online.Where(o => o.Name.Equals(player.Name)).FirstOrDefault();

                    if (dbRecord != null)
                    {
                        dbRecord.Level = player.Level;
                        dbRecord.Vocation = player.Vocation;
                    }
                    else
                    {
                        otContext.Online.Add(new OnlinePlayer
                        {
                            Name = player.Name,
                            Level = player.Level,
                            Vocation = player.Vocation,
                        });
                    }

                    if (currentRemove.ContainsKey(player.Name))
                    {
                        currentRemove.Remove(player.Name);
                    }
                }

                foreach (var player in currentRemove.Values)
                {
                    otContext.Online.Remove(player);
                }

                otContext.SaveChanges();

                this.ResponsePackets.Add(new CreatePlayerListResultPacket(isNewRecord));
            }
        }
    }
}