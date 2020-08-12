// -----------------------------------------------------------------
// <copyright file="SquarePacket.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a square around a creature packet.
    /// </summary>
    public class SquarePacket : IOutboundPacket
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
    }
}
