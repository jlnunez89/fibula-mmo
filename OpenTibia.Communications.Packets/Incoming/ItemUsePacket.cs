// -----------------------------------------------------------------
// <copyright file="ItemUsePacket.cs" company="2Dudes">
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
    /// Class that represents a packet for an item use.
    /// </summary>
    public class ItemUsePacket : IIncomingPacket, IItemUseInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemUsePacket"/> class.
        /// </summary>
        /// <param name="fromLocation">The location from which the item is being used.</param>
        /// <param name="clientId">The id of the item as seen by the client.</param>
        /// <param name="fromStackPos">The position in the stack from which the item is being used.</param>
        /// <param name="index">The index of the item being used.</param>
        public ItemUsePacket(Location fromLocation, ushort clientId, byte fromStackPos, byte index)
        {
            this.ItemClientId = clientId;

            this.FromLocation = fromLocation;
            this.FromStackPos = fromStackPos;

            this.Index = index;
        }

        /// <summary>
        /// Gets the location from which the item is being used.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Gets the position in the stack from which the item is being used.
        /// </summary>
        public byte FromStackPos { get; }

        /// <summary>
        /// Gets the id of the item.
        /// </summary>
        public ushort ItemClientId { get; }

        /// <summary>
        /// Gets the index of the item being used.
        /// </summary>
        public byte Index { get; }
    }
}
