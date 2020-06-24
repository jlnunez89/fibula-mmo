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
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a packet for when a creature speaks.
    /// </summary>
    public class CreatureSpeechPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureSpeechPacket"/> class.
        /// </summary>
        /// <param name="senderId">The id of the creature that spoke.</param>
        /// <param name="senderName">The name of the creature that spoke.</param>
        /// <param name="speechType">The type of speech.</param>
        /// <param name="text">The content of the speech.</param>
        /// <param name="location">The location at which the speech originated.</param>
        /// <param name="channelType">The type of channel in which the speech happened.</param>
        /// <param name="time">The time at which the speech happened.</param>
        public CreatureSpeechPacket(uint senderId, string senderName, SpeechType speechType, string text, Location location, ChatChannelType channelType, uint time)
        {
            this.SenderId = senderId;
            this.SenderName = senderName;
            this.SpeechType = speechType;
            this.Text = text;
            this.Location = location;
            this.Channel = channelType;
            this.Time = time;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.CreatureSpeech;

        /// <summary>
        /// Gets the id of the creature that spoke.
        /// </summary>
        public uint SenderId { get; }

        /// <summary>
        /// Gets the name of the creature that spoke.
        /// </summary>
        public string SenderName { get; }

        /// <summary>
        /// Gets the type of speech.
        /// </summary>
        public SpeechType SpeechType { get; }

        /// <summary>
        /// Gets the content of the speech.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the location at which the speech originated.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the type of channel in which the speech happened.
        /// </summary>
        public ChatChannelType Channel { get; }

        /// <summary>
        /// Gets the time at which the speech happened.
        /// </summary>
        public uint Time { get; }
    }
}
