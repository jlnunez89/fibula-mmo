// -----------------------------------------------------------------
// <copyright file="AnimatedTextPacket.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Outgoing
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a packet with information about animated text that should be displayed to the player.
    /// </summary>
    public class AnimatedTextPacket : IOutboundPacket
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
    }
}
