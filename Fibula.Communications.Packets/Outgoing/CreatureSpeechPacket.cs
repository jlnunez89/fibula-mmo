// -----------------------------------------------------------------
// <copyright file="CreatureSpeechPacket.cs" company="2Dudes">
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
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Server.Contracts.Structs;

    public class CreatureSpeechPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureSpeechPacket"/> class.
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="senderName"></param>
        /// <param name="speechType"></param>
        /// <param name="text"></param>
        /// <param name="location"></param>
        /// <param name="channelId"></param>
        /// <param name="time"></param>
        public CreatureSpeechPacket(uint senderId, string senderName, SpeechType speechType, string text, Location location, ChatChannelType channelId, uint time)
        {
            this.SenderId = senderId;
            this.SenderName = senderName;
            this.SpeechType = speechType;
            this.Text = text;
            this.Location = location;
            this.ChannelId = channelId;
            this.Time = time;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.CreatureSpeech;

        public uint SenderId { get; }

        public string SenderName { get; }

        public SpeechType SpeechType { get; }

        public string Text { get; }

        public Location Location { get; }

        public ChatChannelType ChannelId { get; }

        public uint Time { get; }
    }
}
