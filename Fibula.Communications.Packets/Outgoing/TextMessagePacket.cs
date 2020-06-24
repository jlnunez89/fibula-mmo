// -----------------------------------------------------------------
// <copyright file="TextMessagePacket.cs" company="2Dudes">
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
    /// Class that represents a text message packet.
    /// </summary>
    public class TextMessagePacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessagePacket"/> class.
        /// </summary>
        /// <param name="type">The messsage type.</param>
        /// <param name="message">The message.</param>
        public TextMessagePacket(MessageType type, string message)
        {
            this.Type = type;
            this.Message = message;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.TextMessage;

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public MessageType Type { get; }

        /// <summary>
        /// Gets the message contents.
        /// </summary>
        public string Message { get; }
    }
}
