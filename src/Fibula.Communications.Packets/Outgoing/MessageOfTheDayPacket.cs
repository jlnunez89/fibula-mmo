// -----------------------------------------------------------------
// <copyright file="MessageOfTheDayPacket.cs" company="2Dudes">
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
    /// Class that represents an outgoing message of the day packet.
    /// </summary>
    public sealed class MessageOfTheDayPacket : IOutboundPacket
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
        public OutgoingPacketType PacketType => OutgoingPacketType.MessageOfTheDay;

        /// <summary>
        /// Gets the message of the day.
        /// </summary>
        public string MessageOfTheDay { get; }
    }
}
