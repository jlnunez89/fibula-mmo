// <copyright file="CreatePlayerListPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a created player list packet.
    /// </summary>
    public class CreatePlayerListPacket : IIncomingPacket, IOnlinePlayerListInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePlayerListPacket"/> class.
        /// </summary>
        /// <param name="playerList">The players list.</param>
        public CreatePlayerListPacket(IList<IOnlinePlayer> playerList)
        {
            playerList.ThrowIfNull(nameof(playerList));

            this.PlayerList = playerList;
        }

        /// <summary>
        /// Gets the player list.
        /// </summary>
        public IList<IOnlinePlayer> PlayerList { get; }
    }
}
