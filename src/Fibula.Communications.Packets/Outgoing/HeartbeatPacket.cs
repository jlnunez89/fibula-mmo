// -----------------------------------------------------------------
// <copyright file="HeartbeatPacket.cs" company="2Dudes">
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
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a packet for cancelling a player's walk.
    /// </summary>
    public class HeartbeatPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeartbeatPacket"/> class.
        /// </summary>
        public HeartbeatPacket()
        {
            // TODO: take the OS version here to adjust the packet to send later.
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public OutgoingPacketType PacketType => OutgoingPacketType.Heartbeat;
    }
}
