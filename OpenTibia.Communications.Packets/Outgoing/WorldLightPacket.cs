// -----------------------------------------------------------------
// <copyright file="WorldLightPacket.cs" company="2Dudes">
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
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a world light packet.
    /// </summary>
    public class WorldLightPacket : IOutgoingPacket
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

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteWorldLightPacket(this);
        }
    }
}
