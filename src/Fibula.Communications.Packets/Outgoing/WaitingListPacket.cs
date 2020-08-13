// -----------------------------------------------------------------
// <copyright file="WaitingListPacket.cs" company="2Dudes">
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
    /// Class that represents a waiting list packet.
    /// </summary>
    public class WaitingListPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitingListPacket"/> class.
        /// </summary>
        /// <param name="message">The message to send along.</param>
        /// <param name="waitTimeInSeconds">The wait time in seconds to instruct the client to wait before re-checking.</param>
        public WaitingListPacket(string message, byte waitTimeInSeconds)
        {
            this.Message = message;
            this.WaitTime = waitTimeInSeconds;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public OutgoingPacketType PacketType => OutgoingPacketType.WaitingList;

        /// <summary>
        /// Gets the message to send.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the wait time in seconds to instruct the client to wait before re-checking.
        /// </summary>
        public byte WaitTime { get; }
    }
}
