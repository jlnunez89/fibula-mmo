// -----------------------------------------------------------------
// <copyright file="WorldLightPacket.cs" company="2Dudes">
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
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a world light packet.
    /// </summary>
    public class WorldLightPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorldLightPacket"/> class.
        /// </summary>
        /// <param name="level">The light level.</param>
        /// <param name="color">The color level.</param>
        public WorldLightPacket(byte level, byte color)
        {
            this.Level = level;
            this.Color = color;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.WorldLight;

        /// <summary>
        /// Gets the level of the light.
        /// </summary>
        public byte Level { get; }

        /// <summary>
        /// Gets the color of the light.
        /// </summary>
        public byte Color { get; }
    }
}
