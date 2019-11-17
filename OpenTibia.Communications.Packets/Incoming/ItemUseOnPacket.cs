// <copyright file="ItemUseOnPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    public class ItemUseOnPacket : IIncomingPacket, IItemUseOnInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemUseOnPacket"/> class.
        /// </summary>
        /// <param name="fromLocation"></param>
        /// <param name="fromSpriteId"></param>
        /// <param name="fromStackPos"></param>
        /// <param name="toLocation"></param>
        /// <param name="toSpriteId"></param>
        /// <param name="toStackPos"></param>
        public ItemUseOnPacket(Location fromLocation, ushort fromSpriteId, byte fromStackPos, Location toLocation, ushort toSpriteId, byte toStackPos)
        {
            this.FromLocation = fromLocation;
            this.FromSpriteId = fromSpriteId;
            this.FromStackPosition = fromStackPos;

            this.ToLocation = toLocation;
            this.ToSpriteId = toSpriteId;
            this.ToStackPosition = toStackPos;
        }

        public Location FromLocation { get; }

        public ushort FromSpriteId { get; }

        public byte FromStackPosition { get; }

        public Location ToLocation { get; }

        public ushort ToSpriteId { get; }

        public byte ToStackPosition { get; }
    }
}
