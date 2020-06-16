﻿// -----------------------------------------------------------------
// <copyright file="MessageOfTheDayPacket.cs" company="2Dudes">
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
        public byte PacketType => (byte)OutgoingGatewayPacketType.MessageOfTheDay;

        /// <summary>
        /// Gets the message of the day.
        /// </summary>
        public string MessageOfTheDay { get; }
    }
}
