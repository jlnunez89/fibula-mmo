// -----------------------------------------------------------------
// <copyright file="MapDescriptionPacket.cs" company="2Dudes">
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
    /// Class that represents a map description packet.
    /// </summary>
    public class MapDescriptionPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapDescriptionPacket"/> class.
        /// </summary>
        /// <param name="origin">The origin location.</param>
        /// <param name="descriptionBytes">The description bytes.</param>
        public MapDescriptionPacket(Location origin, ReadOnlySequence<byte> descriptionBytes)
        {
            this.Origin = origin;
            this.DescriptionBytes = descriptionBytes;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.MapDescription;

        /// <summary>
        /// Gets the origin location.
        /// </summary>
        public Location Origin { get; }

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
            message.WriteMapDescriptionPacket(this);
        }
    }
}
