// -----------------------------------------------------------------
// <copyright file="MagicEffectPacket.cs" company="2Dudes">
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
    /// Class that represents a magic effect packet.
    /// </summary>
    public class MagicEffectPacket : IOutboundPacket
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
    }
}
