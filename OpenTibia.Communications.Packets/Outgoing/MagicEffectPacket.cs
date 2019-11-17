// <copyright file="MagicEffectPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Data.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    public class MagicEffectPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MagicEffectPacket"/> class.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="effect"></param>
        public MagicEffectPacket(Location location, AnimatedEffect effect)
        {
            this.Location = location;
            this.Effect = effect;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.MagicEffect;

        public Location Location { get; }

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
