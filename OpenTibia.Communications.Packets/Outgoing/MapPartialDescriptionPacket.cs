﻿// -----------------------------------------------------------------
// <copyright file="MapPartialDescriptionPacket.cs" company="2Dudes">
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
    using System;
    using System.Buffers;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a partial map description packet.
    /// </summary>
    public class MapPartialDescriptionPacket : IOutgoingPacket
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

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteMapPartialDescriptionPacket(this);
        }
    }
}
