// -----------------------------------------------------------------
// <copyright file="IItemMoveInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Interface for an item movement information.
    /// </summary>
    public interface IItemMoveInfo
    {
        /// <summary>
        /// Gets the id of the item, as seen by the client.
        /// </summary>
        ushort ClientId { get; }

        /// <summary>
        /// Gets the location from which the item is being moved.
        /// </summary>
        Location FromLocation { get; }

        /// <summary>
        /// Gets the position in the stack at the location from which the item is being moved.
        /// </summary>
        byte FromStackPos { get; }

        /// <summary>
        /// Gets the location to which the item is being moved.
        /// </summary>
        Location ToLocation { get; }

        /// <summary>
        /// Gets the amount of the item being moved.
        /// </summary>
        byte Count { get; }
    }
}