// <copyright file="IOnlinePlayerListInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for player list information.
    /// </summary>
    public interface IOnlinePlayerListInfo
    {
        /// <summary>
        /// Gets the player list.
        /// </summary>
        IList<IOnlinePlayer> PlayerList { get; }
    }
}