// -----------------------------------------------------------------
// <copyright file="IRotateItemInfo.cs" company="2Dudes">
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
    /// Interface for an item rotation information.
    /// </summary>
    public interface IRotateItemInfo
    {
        /// <summary>
        /// Gets the location form which the item is being rotated.
        /// </summary>
        Location AtLocation { get; }

        /// <summary>
        /// Gets the id of the item as seen by the client.
        /// </summary>
        ushort ItemClientId { get; }

        /// <summary>
        /// Gets the index of the item being rotated.
        /// </summary>
        byte Index { get; }
    }
}