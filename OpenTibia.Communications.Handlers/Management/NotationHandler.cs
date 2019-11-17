// <copyright file="NotationHandler.cs" company="2Dudes">
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

    public class NotationHandler : BaseHandler
    {
        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingManagementPacketType.Notation;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            var ruleViolationInfo = message.ReadRuleViolationInfo();

            using (var otContext = new OpenTibiaDbContext())
            {
                var playerRecord = otContext.Players.Where(p => p.Charname.Equals(ruleViolationInfo.CharacterName)).FirstOrDefault();

                if (playerRecord != null)
                {
                    var userRecord = otContext.Users.Where(u => u.Login == playerRecord.Account_Nr).FirstOrDefault();

                    if (userRecord != null)
                    {
                        var nowUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                        otContext.Banishments.Add(new Banishment
                        {
                            AccountId = playerRecord.Account_Id,
                            AccountNr = playerRecord.Account_Nr,
                            Ip = ruleViolationInfo.IpAddress,
                            GmId = ruleViolationInfo.GamemasterId,
                            Violation = ruleViolationInfo.Reason,
                            Comment = ruleViolationInfo.Comment,
                            Timestamp = nowUnixTimestamp,
                            BanishedUntil = nowUnixTimestamp,
                            PunishmentType = 0x02,
                        });

                        otContext.SaveChanges();

                        this.ResponsePackets.Add(new NotationResultPacket(ruleViolationInfo.GamemasterId));

                        return;
                    }
                }
            }

            this.ResponsePackets.Add(new DefaultErrorPacket());
        }
    }
}