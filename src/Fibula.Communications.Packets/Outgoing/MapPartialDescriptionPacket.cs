// -----------------------------------------------------------------
// <copyright file="MapPartialDescriptionPacket.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
        public MapPartialDescriptionPacket(OutgoingPacketType mapDescriptionType, ReadOnlySequence<byte> descriptionBytes)
        {
            if (mapDescriptionType != OutgoingPacketType.MapSliceEast &&
                mapDescriptionType != OutgoingPacketType.MapSliceNorth &&
                mapDescriptionType != OutgoingPacketType.MapSliceSouth &&
                mapDescriptionType != OutgoingPacketType.MapSliceWest &&
                mapDescriptionType != OutgoingPacketType.FloorChangeUp &&
                mapDescriptionType != OutgoingPacketType.FloorChangeDown)
            {
                throw new ArgumentException(nameof(mapDescriptionType));
            }

            this.PacketType = mapDescriptionType;
            this.DescriptionBytes = descriptionBytes;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public OutgoingPacketType PacketType { get; }

        /// <summary>
        /// Gets the description bytes.
        /// </summary>
        public ReadOnlySequence<byte> DescriptionBytes { get; }
    }
}
