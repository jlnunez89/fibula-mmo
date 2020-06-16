// -----------------------------------------------------------------
// <copyright file="MapPartialDescriptionPacket.cs" company="2Dudes">
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
    using System;
    using System.Buffers;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a partial map description packet.
    /// </summary>
    public class MapPartialDescriptionPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapPartialDescriptionPacket"/> class.
        /// </summary>
        /// <param name="mapDescriptionType">The type of map description.</param>
        /// <param name="descriptionBytes">The description bytes.</param>
        public MapPartialDescriptionPacket(OutgoingGamePacketType mapDescriptionType, ReadOnlySequence<byte> descriptionBytes)
        {
            if (mapDescriptionType != OutgoingGamePacketType.MapSliceEast &&
                mapDescriptionType != OutgoingGamePacketType.MapSliceNorth &&
                mapDescriptionType != OutgoingGamePacketType.MapSliceSouth &&
                mapDescriptionType != OutgoingGamePacketType.MapSliceWest &&
                mapDescriptionType != OutgoingGamePacketType.FloorChangeUp &&
                mapDescriptionType != OutgoingGamePacketType.FloorChangeDown)
            {
                throw new ArgumentException(nameof(mapDescriptionType));
            }

            this.PacketType = (byte)mapDescriptionType;
            this.DescriptionBytes = descriptionBytes;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType { get; }

        /// <summary>
        /// Gets the description bytes.
        /// </summary>
        public ReadOnlySequence<byte> DescriptionBytes { get; }
    }
}
