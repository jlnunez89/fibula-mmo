// -----------------------------------------------------------------
// <copyright file="AnimatedTextPacket.cs" company="2Dudes">
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
