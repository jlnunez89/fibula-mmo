// -----------------------------------------------------------------
// <copyright file="PlayerCancelWalkPacket.cs" company="2Dudes">
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
        public byte PacketType => (byte)OutgoingGamePacketType.CancelWalk;

        /// <summary>
        /// Gets the direction in which the creature will be left facing.
        /// </summary>
        public Direction ResultingDirection { get; }
    }
}
