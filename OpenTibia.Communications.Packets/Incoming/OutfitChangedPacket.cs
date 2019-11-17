// <copyright file="OutfitChangedPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    public class OutfitChangedPacket : IIncomingPacket, IOutfitInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutfitChangedPacket"/> class.
        /// </summary>
        /// <param name="newOutfit"></param>
        public OutfitChangedPacket(Outfit newOutfit)
        {
            this.Outfit = newOutfit;
        }

        public Outfit Outfit { get; }
    }
}
