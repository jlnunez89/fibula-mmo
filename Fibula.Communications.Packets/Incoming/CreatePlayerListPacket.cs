// -----------------------------------------------------------------
// <copyright file="CreatePlayerListPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Incoming
{
    using System.Collections.Generic;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
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
