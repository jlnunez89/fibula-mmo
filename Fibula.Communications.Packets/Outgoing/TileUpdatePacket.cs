// -----------------------------------------------------------------
// <copyright file="TileUpdatePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Outgoing
{
    using System.Buffers;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents a tile update packet.
    /// </summary>
    public class TileUpdatePacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileUpdatePacket"/> class.
        /// </summary>
        /// <param name="location">The location of the tile.</param>
        /// <param name="descriptionBytes">The description bytes of the tile.</param>
        public TileUpdatePacket(Location location, ReadOnlySequence<byte> descriptionBytes)
        {
            this.Location = location;
            this.DescriptionBytes = descriptionBytes;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.TileUpdate;

        /// <summary>
        /// Gets the tile location.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the description bytes.
        /// </summary>
        public ReadOnlySequence<byte> DescriptionBytes { get; }
    }
}
