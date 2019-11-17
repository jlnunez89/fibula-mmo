// <copyright file="CreatureSpeechPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    public class CreatureSpeechPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureSpeechPacket"/> class.
        /// </summary>
        /// <param name="senderName"></param>
        /// <param name="speechType"></param>
        /// <param name="text"></param>
        /// <param name="location"></param>
        /// <param name="channelId"></param>
        /// <param name="time"></param>
        public CreatureSpeechPacket(string senderName, SpeechType speechType, string text, Location location, ChatChannelType channelId, uint time)
        {
            this.SenderName = senderName;
            this.SpeechType = speechType;
            this.Text = text;
            this.Location = location;
            this.ChannelId = channelId;
            this.Time = time;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.CreatureSpeech;

        public string SenderName { get; }

        public SpeechType SpeechType { get; }

        public string Text { get; }

        public Location Location { get; }

        public ChatChannelType ChannelId { get; }

        public uint Time { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteCreatureSpeechPacket(this);
        }
    }
}
