// -----------------------------------------------------------------
// <copyright file="WaitingListPacket.cs" company="2Dudes">
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
        public byte PacketType => (byte)OutgoingGamePacketType.WaitingList;

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
