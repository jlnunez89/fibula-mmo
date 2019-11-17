// -----------------------------------------------------------------
// <copyright file="PlayerLoggedOutHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Handlers.Management
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Outgoing;

    public class PlayerLoggedOutHandler : BaseHandler
    {
        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingManagementPacketType.PlayerLogOut;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            var playerLogoutInfo = message.ReadManagementPlayerLogoutInfo();

            using (var otContext = new OpenTibiaDbContext())
            {
                var playerRecord = otContext.Players.Where(p => p.Account_Id == playerLogoutInfo.AccountId).FirstOrDefault();

                if (playerRecord != null)
                {
                    playerRecord.Level = playerLogoutInfo.Level;
                    playerRecord.Vocation = playerLogoutInfo.Vocation;
                    playerRecord.Residence = playerLogoutInfo.Residence;
                    playerRecord.Lastlogin = playerLogoutInfo.LastLogin;

                    playerRecord.Online = 0;

                    var onlineRecord = otContext.Online.Where(o => o.Name.Equals(playerRecord.Charname)).FirstOrDefault();

                    if (onlineRecord != null)
                    {
                        otContext.Online.Remove(onlineRecord);
                    }

                    otContext.SaveChanges();

                    this.ResponsePackets.Add(new DefaultNoErrorPacket());
                }
            }
        }
    }
}