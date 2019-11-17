// <copyright file="PlayerLoginSucessPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using System.Collections.Generic;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    public class PlayerLoginSucessPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLoginSucessPacket"/> class.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="characterName"></param>
        /// <param name="gender"></param>
        /// <param name="guild"></param>
        /// <param name="guildTitle"></param>
        /// <param name="playerTitle"></param>
        /// <param name="vipContacts"></param>
        /// <param name="premiumDays"></param>
        /// <param name="recentlyActivatedPremmium"></param>
        /// <param name="privileges"></param>
        public PlayerLoginSucessPacket(int accountId, string characterName, byte gender, string guild, string guildTitle, string playerTitle, IList<IVipContact> vipContacts, int premiumDays, bool recentlyActivatedPremmium, HashSet<string> privileges)
        {
            this.AccountId = accountId;
            this.CharacterName = characterName;
            this.Gender = gender;
            this.Guild = guild;
            this.GuildTitle = guildTitle;
            this.PlayerTitle = playerTitle;
            this.VipContacts = vipContacts;
            this.PremiumDays = premiumDays;
            this.RecentlyActivatedPremmium = recentlyActivatedPremmium;
            this.Privileges = privileges;
        }

        public byte PacketType => (byte)OutgoingManagementPacketType.NoError;

        public int AccountId { get; }

        public string CharacterName { get; }

        public byte Gender { get; }

        public string Guild { get; }

        public string GuildTitle { get; }

        public string PlayerTitle { get; }

        public IList<IVipContact> VipContacts { get; }

        public int PremiumDays { get; }

        public bool RecentlyActivatedPremmium { get; }

        public HashSet<string> Privileges { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WritePlayerLoginSucessPacket(this);
        }
    }
}
