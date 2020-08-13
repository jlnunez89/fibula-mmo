// -----------------------------------------------------------------
// <copyright file="SpeechPacket.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Incoming
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a speech packet.
    /// </summary>
    public class SpeechPacket : IIncomingPacket, ISpeechInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechPacket"/> class.
        /// </summary>
        /// <param name="type">The type of speech.</param>
        /// <param name="channelId">The channel type.</param>
        /// <param name="content">The content spoken.</param>
        /// <param name="receiver">Optional. The receiver of the message, if any.</param>
        public SpeechPacket(SpeechType type, ChatChannelType channelId, string content, string receiver = "")
        {
            this.SpeechType = type;
            this.ChannelType = channelId;
            this.Content = content;
            this.Receiver = receiver;
        }

        /// <summary>
        /// Gets the type of speech.
        /// </summary>
        public SpeechType SpeechType { get; }

        /// <summary>
        /// Gets the channel type.
        /// </summary>
        public ChatChannelType ChannelType { get; }

        /// <summary>
        /// Gets the content of the message.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the receiver of the message.
        /// </summary>
        public string Receiver { get; }
    }
}
