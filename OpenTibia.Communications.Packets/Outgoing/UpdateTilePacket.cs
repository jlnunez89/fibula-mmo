// -----------------------------------------------------------------
// <copyright file="UpdateTilePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Outgoing
{
    using System.Buffers;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents a tile update packet.
    /// </summary>
    public class UpdateTilePacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateTilePacket"/> class.
        /// </summary>
        /// <param name="location">The location of the tile.</param>
        /// <param name="descriptionBytes">The description bytes of the tile.</param>
        public UpdateTilePacket(Location location, ReadOnlySequence<byte> descriptionBytes)
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

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteUpdateTilePacket(this);
        }
    }
}
