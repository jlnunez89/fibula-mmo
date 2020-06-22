// -----------------------------------------------------------------
// <copyright file="SelfAppearPacketWriter.cs" company="2Dudes">
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
    using System;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Serilog;

    /// <summary>
    /// Class that represents a self appear packet writer for the game server.
    /// </summary>
    public class SelfAppearPacketWriter : BasePacketWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfAppearPacketWriter"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public SelfAppearPacketWriter(ILogger logger)
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
            if (!(packet is SelfAppearPacket selfAppearPacket))
            {
                this.Logger.Warning($"Invalid packet {packet.GetType().Name} routed to {this.GetType().Name}");

                return;
            }

            message.AddByte(selfAppearPacket.PacketType);

            message.AddUInt32(selfAppearPacket.CreatureId);
            message.AddByte(selfAppearPacket.GraphicsSpeed);
            message.AddByte(selfAppearPacket.CanReportBugs);

            message.AddByte(Math.Min((byte)0x01, selfAppearPacket.Player.PermissionsLevel));

            if (selfAppearPacket.Player.PermissionsLevel > 0)
            {
                // TODO: WTF are these, permissions flags?
                message.AddByte(0x0B);

                for (var i = 0; i < 32; i++)
                {
                    message.AddByte(0xFF);
                }
            }
        }
    }
}
