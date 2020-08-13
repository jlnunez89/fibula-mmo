// -----------------------------------------------------------------
// <copyright file="CreatureTurnedPacketWriter.cs" company="2Dudes">
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
    /// Class that represents a creature turned packet writer for the game server.
    /// </summary>
    public class CreatureTurnedPacketWriter : BasePacketWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureTurnedPacketWriter"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public CreatureTurnedPacketWriter(ILogger logger)
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
            if (!(packet is CreatureTurnedPacket creatureTurnedPacket))
            {
                this.Logger.Warning($"Invalid packet {packet.GetType().Name} routed to {this.GetType().Name}");

                return;
            }

            message.AddByte(creatureTurnedPacket.PacketType.ToByte());

            message.AddLocation(creatureTurnedPacket.Creature.Location);
            message.AddByte(creatureTurnedPacket.StackPosition);
            message.AddUInt16(creatureTurnedPacket.Creature.TypeId);
            message.AddUInt32(creatureTurnedPacket.Creature.Id);
            message.AddByte((byte)creatureTurnedPacket.Creature.Direction);
        }
    }
}
