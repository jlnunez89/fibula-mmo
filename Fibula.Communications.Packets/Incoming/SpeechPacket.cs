// -----------------------------------------------------------------
// <copyright file="SpeechPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Incoming
{
    using Fibula.Communications.Packets.Contracts.Abstractions;
    using Fibula.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a speech packet.
    /// </summary>
    public class SpeechPacket : ISpeechInfo
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
