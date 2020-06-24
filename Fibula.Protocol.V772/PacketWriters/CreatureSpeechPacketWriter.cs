// -----------------------------------------------------------------
// <copyright file="CreatureSpeechPacketWriter.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Protocol.V772.PacketWriters
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Serilog;

    /// <summary>
    /// Class that represents a creature speech packet writer for the game server.
    /// </summary>
    public class CreatureSpeechPacketWriter : BasePacketWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureSpeechPacketWriter"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public CreatureSpeechPacketWriter(ILogger logger)
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
            if (!(packet is CreatureSpeechPacket creatureSpeechPacket))
            {
                this.Logger.Warning($"Invalid packet {packet.GetType().Name} routed to {this.GetType().Name}");

                return;
            }

            message.AddByte(creatureSpeechPacket.PacketType);

            message.AddUInt32(0);
            message.AddString(creatureSpeechPacket.SenderName);
            message.AddByte((byte)creatureSpeechPacket.SpeechType);

            switch (creatureSpeechPacket.SpeechType)
            {
                case SpeechType.Say:
                case SpeechType.Whisper:
                case SpeechType.Yell:
                case SpeechType.MonsterSay:
                // case SpeechType.MonsterYell:
                    message.AddLocation(creatureSpeechPacket.Location);
                    break;

                // case SpeechType.ChannelRed:
                // case SpeechType.ChannelRedAnonymous:
                // case SpeechType.ChannelOrange:
                case SpeechType.ChannelYellow:
                    // case SpeechType.ChannelWhite:
                    message.AddUInt16((ushort)creatureSpeechPacket.Channel);
                    break;

                case SpeechType.RuleViolationReport:
                    message.AddUInt32(creatureSpeechPacket.Time);
                    break;

                default:
                    break;
            }

            message.AddString(creatureSpeechPacket.Text);
        }
    }
}
