// <copyright file="BanismentHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers.Management
{
    using System;
    using System.Linq;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data;

    public class BanismentHandler : BaseHandler
    {
        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingManagementPacketType.Banishment;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public override void HandleRequest(INetworkMessage message, IConnection connection)
        {
            var ruleViolationInfo = message.ReadRuleViolationInfo();

            byte banDays = 0;
            var banUntilDate = DateTimeOffset.UtcNow;

            using (var unitOfWork = new UnitOfWork(new OpenTibiaContext()))
            {
                var character = unitOfWork.Characters.Find(c => c.Name.Equals(ruleViolationInfo.CharacterName)).FirstOrDefault();

                if (character != null)
                {
                    var account = unitOfWork.Accounts.Find(a => a.Id.Equals(character.AccountId)).FirstOrDefault();

                    if (account != null)
                    {
                        // Calculate Banishment date based on number of previous banishments youger than 60 days...
                        var todayMinus60Days = DateTimeOffset.UtcNow.AddDays(-60);
                        var banCount = unitOfWork.Banishments.Where(b => b.AccountNr == character.AccountId && b.Timestamp > todayMinus60Days && b.PunishmentType == 1).Count();

                        switch (banCount)
                        {
                            case 0:
                                banDays = 7;
                                break;
                            case 1:
                                banDays = 15;
                                break;
                            case 2:
                                banDays = 30;
                                break;
                            case 3:
                                banDays = 90;
                                break;
                            default:
                                banDays = 255;
                                break;
                        }

                        banUntilDate = banUntilDate.AddDays(banDays);
                        var nowUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        var banUntilUnixTimestamp = banUntilDate.ToUnixTimeSeconds();

                        unitOfWork.Banishments.Add(
                            new Banishment
                            {
                                AccountId = character.AccountId,
                                Ip = ruleViolationInfo.IpAddress,
                                GmId = ruleViolationInfo.GamemasterId,
                                Violation = ruleViolationInfo.Reason,
                                Comment = ruleViolationInfo.Comment,
                                Timestamp = nowUnixTimestamp,
                                BanishedUntil = banUntilUnixTimestamp,
                                PunishmentType = 1,
                            });

                        account.Banished = true;
                        account.BanishedUntil = banUntilDate;

                        unitOfWork.Complete();

                        this.ResponsePackets.Add(new BanismentResultPacket(banDays, (uint)banUntilDate.ToUnixTimeSeconds()));

                        return;
                    }
                }
            }

            this.ResponsePackets.Add(new DefaultErrorPacket());
        }
    }
}