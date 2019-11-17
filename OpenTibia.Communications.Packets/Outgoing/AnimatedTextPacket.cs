// <copyright file="AnimatedTextPacket.cs" company="2Dudes">
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

    /// <summary>
    /// Class that represents a packet with information about animated text that should be displayed to the user.
    /// </summary>
    public class AnimatedTextPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedTextPacket"/> class.
        /// </summary>
        /// <param name="location">The location of the animated text.</param>
        /// <param name="color">The text color.</param>
        /// <param name="text">The contents of the text.</param>
        public AnimatedTextPacket(Location location, TextColor color, string text)
        {
            this.Location = location;
            this.Color = color;
            this.Text = text;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.AnimatedText;

        /// <summary>
        /// Gets the location of the animated text.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the text color.
        /// </summary>
        public TextColor Color { get; }

        /// <summary>
        /// Gets the content of the text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteAnimatedTextPacket(this);
        }
    }
}
