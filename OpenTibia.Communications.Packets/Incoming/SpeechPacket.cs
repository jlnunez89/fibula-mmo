// -----------------------------------------------------------------
// <copyright file="SpeechPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    public class SpeechPacket : IIncomingPacket, ISpeechInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechPacket"/> class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="channelId"></param>
        /// <param name="content"></param>
        /// <param name="receiver"></param>
        public SpeechPacket(SpeechType type, ChatChannelType channelId, string content, string receiver = "")
        {
            this.Type = type;
            this.ChannelId = channelId;
            this.Content = content;
            this.Receiver = receiver;
        }

        public SpeechType Type { get; }

        public ChatChannelType ChannelId { get; }

        public string Receiver { get; }

        public string Content { get; }
    }
}
