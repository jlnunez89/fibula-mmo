// -----------------------------------------------------------------
// <copyright file="OutfitChangedPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Incoming
{
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Contracts.Abstractions;
    using Fibula.Server.Contracts.Structs;

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
