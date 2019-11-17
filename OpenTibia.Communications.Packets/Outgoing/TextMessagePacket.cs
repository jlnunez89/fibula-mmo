// <copyright file="TextMessagePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Enumerations;

    public class TextMessagePacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessagePacket"/> class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public TextMessagePacket(MessageType type, string message)
        {
            this.Type = type;
            this.Message = message;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.TextMessage;

        public MessageType Type { get; }

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
