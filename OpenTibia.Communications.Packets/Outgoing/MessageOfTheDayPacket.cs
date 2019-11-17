// -----------------------------------------------------------------
// <copyright file="MessageOfTheDayPacket.cs" company="2Dudes">
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
    /// Class that represents an outgoing message of the day packet.
    /// </summary>
    public sealed class MessageOfTheDayPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOfTheDayPacket"/> class.
        /// </summary>
        /// <param name="messageOfTheDay">The message.</param>
        public MessageOfTheDayPacket(string messageOfTheDay)
        {
            this.MessageOfTheDay = messageOfTheDay;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingManagementPacketType.MessageOfTheDay;

        /// <summary>
        /// Gets the message of the day.
        /// </summary>
        public string MessageOfTheDay { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteMessageOfTheDayPacket(this);
        }
    }
}
