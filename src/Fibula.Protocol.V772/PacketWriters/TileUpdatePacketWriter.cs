// -----------------------------------------------------------------
// <copyright file="TileUpdatePacketWriter.cs" company="2Dudes">
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
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Protocol.V772.Extensions;
    using Serilog;

    /// <summary>
    /// Class that represents a tile update packet writer for the game server.
    /// </summary>
    public class TileUpdatePacketWriter : BasePacketWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileUpdatePacketWriter"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public TileUpdatePacketWriter(ILogger logger)
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
            if (!(packet is TileUpdatePacket tileUpdatePacket))
            {
                this.Logger.Warning($"Invalid packet {packet.GetType().Name} routed to {this.GetType().Name}");

                return;
            }

            message.AddByte(tileUpdatePacket.PacketType.ToByte());

            message.AddLocation(tileUpdatePacket.Location);

            if (tileUpdatePacket.DescriptionBytes.Length > 0)
            {
                message.AddBytes(tileUpdatePacket.DescriptionBytes);
                message.AddByte(0x00); // skip count
            }
            else
            {
                message.AddByte(0x01); // skip count
            }

            message.AddByte(byte.MaxValue);
        }
    }
}
