// <copyright file="ItemMovePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    public class ItemMovePacket : IIncomingPacket, IItemMoveInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemMovePacket"/> class.
        /// </summary>
        /// <param name="fromLocation"></param>
        /// <param name="clientId"></param>
        /// <param name="fromStackPos"></param>
        /// <param name="toLocation"></param>
        /// <param name="count"></param>
        public ItemMovePacket(Location fromLocation, ushort clientId, byte fromStackPos, Location toLocation, byte count)
        {
            this.ClientId = clientId;

            this.FromLocation = fromLocation;
            this.FromStackPos = fromStackPos;

            this.ToLocation = toLocation;

            this.Count = count;
        }

        public Location FromLocation { get; }

        public Location ToLocation { get; }

        public byte FromStackPos { get; }

        public ushort ClientId { get; }

        public byte Count { get; }
    }
}
