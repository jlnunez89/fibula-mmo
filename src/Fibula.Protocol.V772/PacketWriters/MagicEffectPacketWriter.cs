// -----------------------------------------------------------------
// <copyright file="MagicEffectPacketWriter.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Protocol.V772.PacketWriters
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Protocol.V772.Extensions;
    using Serilog;

    /// <summary>
    /// Class that represents a magic effect response writer for the game server.
    /// </summary>
    public class MagicEffectPacketWriter : BasePacketWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MagicEffectPacketWriter"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public MagicEffectPacketWriter(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Writes a packet to the given <see cref="INetworkMessage"/>.
        /// </summary>
        /// <param name="packet">The packet to write.</param>
        /// <param name="message">The message to write into.</param>
        public override void WriteToMessage(IOutboundPacket packet, ref INetworkMessage message)
        {
            if (!(packet is MagicEffectPacket magicEffectPacket))
            {
                this.Logger.Warning($"Invalid packet {packet.GetType().Name} routed to {this.GetType().Name}");

                return;
            }

            if (magicEffectPacket.Effect == AnimatedEffect.None)
            {
                return;
            }

            message.AddByte(magicEffectPacket.PacketType.ToByte());

            message.AddLocation(magicEffectPacket.Location);

            byte valueToSend = magicEffectPacket.Effect switch
            {
                AnimatedEffect.XBlood => 0x01,
                AnimatedEffect.RingsBlue => 0x02,
                AnimatedEffect.Puff => 0x03,
                AnimatedEffect.SparkYellow => 0x04,
                AnimatedEffect.DamageExplosion => 0x05,
                AnimatedEffect.DamageMagicMissile => 0x06,
                AnimatedEffect.AreaFlame => 0x07,
                AnimatedEffect.RingsYellow => 0x08,
                AnimatedEffect.RingsGreen => 0x09,
                AnimatedEffect.XGray => 0x0A,
                AnimatedEffect.BubbleBlue => 0x0B,
                AnimatedEffect.DamageEnergy => 0x0C,
                AnimatedEffect.GlitterBlue => 0x0D,
                AnimatedEffect.GlitterRed => 0x0E,
                AnimatedEffect.GlitterGreen => 0x0F,
                AnimatedEffect.Flame => 0x10,
                AnimatedEffect.Poison => 0x11,
                AnimatedEffect.BubbleBlack => 0x12,
                AnimatedEffect.SoundGreen => 0x13,
                AnimatedEffect.SoundRed => 0x14,
                AnimatedEffect.DamageVenomMissile => 0x15,
                AnimatedEffect.SoundYellow => 0x16,
                AnimatedEffect.SoundPurple => 0x17,
                AnimatedEffect.SoundBlue => 0x18,
                AnimatedEffect.SoundWhite => 0x19,
                _ => 0x03,
            };

            message.AddByte(valueToSend);
        }
    }
}
