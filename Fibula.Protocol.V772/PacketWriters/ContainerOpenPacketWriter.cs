// -----------------------------------------------------------------
// <copyright file="ContainerOpenPacketWriter.cs" company="2Dudes">
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
    using System;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Serilog;

    /// <summary>
    /// Class that represents a container open packet writer for the game server.
    /// </summary>
    public class ContainerOpenPacketWriter : BasePacketWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerOpenPacketWriter"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public ContainerOpenPacketWriter(ILogger logger)
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
            if (!(packet is ContainerOpenPacket containerOpenPacket))
            {
                this.Logger.Warning($"Invalid packet {packet.GetType().Name} routed to {this.GetType().Name}");

                return;
            }

            message.AddByte(containerOpenPacket.PacketType);

            message.AddByte(containerOpenPacket.ContainerId);
            message.AddUInt16(containerOpenPacket.TypeId);
            message.AddString(containerOpenPacket.Name);
            message.AddByte(containerOpenPacket.Volume);
            message.AddByte(Convert.ToByte(containerOpenPacket.HasParent ? 0x01 : 0x00));
            message.AddByte(Convert.ToByte(containerOpenPacket.Contents.Count));

            foreach (var item in containerOpenPacket.Contents)
            {
                message.AddItem(item);
            }
        }
    }
}
