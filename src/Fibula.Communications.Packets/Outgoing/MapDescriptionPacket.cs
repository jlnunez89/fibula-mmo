// -----------------------------------------------------------------
// <copyright file="MapDescriptionPacket.cs" company="2Dudes">
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
    using System.Buffers;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a map description packet.
    /// </summary>
    public class MapDescriptionPacket : IOutboundPacket
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
    }
}
