// -----------------------------------------------------------------
// <copyright file="PlayerWalkCancelPacket.cs" company="2Dudes">
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
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a packet for cancelling a player's walk.
    /// </summary>
    public class PlayerWalkCancelPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerWalkCancelPacket"/> class.
        /// </summary>
        /// <param name="turnToDirection">The direction to leave the player facing after cancellation.</param>
        public PlayerWalkCancelPacket(Direction turnToDirection)
        {
            this.ResultingDirection = turnToDirection;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)GameResponseType.WalkCancel;

        /// <summary>
        /// Gets the direction in which the creature will be left facing.
        /// </summary>
        public Direction ResultingDirection { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WritePlayerWalkCancelPacket(this);
        }
    }
}
