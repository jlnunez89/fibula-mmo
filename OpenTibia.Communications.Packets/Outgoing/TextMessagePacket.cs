// -----------------------------------------------------------------
// <copyright file="TextMessagePacket.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a text message packet.
    /// </summary>
    public class TextMessagePacket : IOutgoingPacket
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

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteTextMessagePacket(this);
        }
    }
}
