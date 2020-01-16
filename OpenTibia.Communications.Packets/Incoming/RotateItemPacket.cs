// -----------------------------------------------------------------
// <copyright file="RotateItemPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents a packet for an item rotation.
    /// </summary>
    public class RotateItemPacket : IIncomingPacket, IRotateItemInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateItemPacket"/> class.
        /// </summary>
        /// <param name="fromLocation">The location from which the item is being used.</param>
        /// <param name="clientId">The id of the item as seen by the client.</param>
        /// <param name="index">The index of the item being used.</param>
        public RotateItemPacket(Location fromLocation, ushort clientId, byte index)
        {
            this.ItemClientId = clientId;
            this.AtLocation = fromLocation;
            this.Index = index;
        }

        /// <summary>
        /// Gets the location from which the item is being rotated.
        /// </summary>
        public Location AtLocation { get; }

        /// <summary>
        /// Gets the id of the item.
        /// </summary>
        public ushort ItemClientId { get; }

        /// <summary>
        /// Gets the index of the item being rotated.
        /// </summary>
        public byte Index { get; }
    }
}
