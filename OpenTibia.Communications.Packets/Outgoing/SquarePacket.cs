// -----------------------------------------------------------------
// <copyright file="SquarePacket.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a square around a creature packet.
    /// </summary>
    public class SquarePacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SquarePacket"/> class.
        /// </summary>
        /// <param name="onCreatureId">The Id of the creature on which to draw the square.</param>
        /// <param name="color">The color of the square to draw.</param>
        public SquarePacket(uint onCreatureId, SquareColor color)
        {
            this.OnCreatureId = onCreatureId;
            this.Color = color;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.Square;

        /// <summary>
        /// Gets the Id of the creature on which the square goes.
        /// </summary>
        public uint OnCreatureId { get; }

        /// <summary>
        /// Gets the color of the square.
        /// </summary>
        public SquareColor Color { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteCreatureSquarePacket(this);
        }
    }
}
