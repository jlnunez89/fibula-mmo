// -----------------------------------------------------------------
// <copyright file="PlayerCancelWalkPacket.cs" company="2Dudes">
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
    /// Class that represents a packet for cancelling a player's walk.
    /// </summary>
    public class PlayerCancelWalkPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerCancelWalkPacket"/> class.
        /// </summary>
        /// <param name="turnToDirection">The direction to leave the player facing after cancellation.</param>
        public PlayerCancelWalkPacket(Direction turnToDirection)
        {
            this.ResultingDirection = turnToDirection;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public OutgoingPacketType PacketType => OutgoingPacketType.CancelWalk;

        /// <summary>
        /// Gets the direction in which the creature will be left facing.
        /// </summary>
        public Direction ResultingDirection { get; }
    }
}
