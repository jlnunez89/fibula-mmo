// <copyright file="WorldLightPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    public class WorldLightPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorldLightPacket"/> class.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="color"></param>
        public WorldLightPacket(byte level, byte color)
        {
            this.Level = level;
            this.Color = color;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.WorldLight;

        public byte Level { get; }

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
