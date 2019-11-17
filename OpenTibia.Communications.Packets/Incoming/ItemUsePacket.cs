// <copyright file="ItemUsePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    public class ItemUsePacket : IIncomingPacket, IItemUseInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemUsePacket"/> class.
        /// </summary>
        /// <param name="fromLocation"></param>
        /// <param name="clientId"></param>
        /// <param name="fromStackPos"></param>
        /// <param name="index"></param>
        public ItemUsePacket(Location fromLocation, ushort clientId, byte fromStackPos, byte index)
        {
            this.ClientId = clientId;

            this.FromLocation = fromLocation;
            this.FromStackPos = fromStackPos;

            this.Index = index;
        }

        public Location FromLocation { get; }

        public byte FromStackPos { get; }

        public ushort ClientId { get; }

        public byte Index { get; }
    }
}
