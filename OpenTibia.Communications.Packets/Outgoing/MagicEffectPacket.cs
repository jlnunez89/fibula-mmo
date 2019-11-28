// -----------------------------------------------------------------
// <copyright file="MagicEffectPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents a magic effect packet.
    /// </summary>
    public class MagicEffectPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MagicEffectPacket"/> class.
        /// </summary>
        /// <param name="location">The location of the effect.</param>
        /// <param name="effect">The effect.</param>
        public MagicEffectPacket(Location location, AnimatedEffect effect)
        {
            this.Location = location;
            this.Effect = effect;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.MagicEffect;

        /// <summary>
        /// Gets the location of the effect.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the actual effect.
        /// </summary>
        public AnimatedEffect Effect { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteMagicEffectPacket(this);
        }
    }
}
