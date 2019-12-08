// -----------------------------------------------------------------
// <copyright file="ItemMovePacket.cs" company="2Dudes">
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
    /// Class that represents an item movement packet.
    /// </summary>
    public class ItemMovePacket : IIncomingPacket, IItemMoveInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemMovePacket"/> class.
        /// </summary>
        /// <param name="fromLocation">The location from which the item is being moved.</param>
        /// <param name="clientId">The id of the item being moved.</param>
        /// <param name="fromStackPos">The position in the stack of the item being moved.</param>
        /// <param name="toLocation">The location to which the item is being moved.</param>
        /// <param name="count">The amount of the item being moved.</param>
        public ItemMovePacket(Location fromLocation, ushort clientId, byte fromStackPos, Location toLocation, byte count)
        {
            this.ClientId = clientId;

            this.FromLocation = fromLocation;
            this.FromStackPos = fromStackPos;

            this.ToLocation = toLocation;

            this.Count = count;
        }

        /// <summary>
        /// Gets the id of the item, as seen by the client.
        /// </summary>
        public ushort ClientId { get; }

        /// <summary>
        /// Gets the location from which the item is being moved.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Gets the position in the stack at the location from which the item is being moved.
        /// </summary>
        public byte FromStackPos { get; }

        /// <summary>
        /// Gets the location to which the item is being moved.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Gets the amount of the item being moved.
        /// </summary>
        public byte Count { get; }
    }
}
