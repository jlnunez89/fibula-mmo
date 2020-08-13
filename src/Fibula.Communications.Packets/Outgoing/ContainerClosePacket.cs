// -----------------------------------------------------------------
// <copyright file="ContainerClosePacket.cs" company="2Dudes">
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
    /// Class that represents a packet for a container being closed.
    /// </summary>
    public class ContainerClosePacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerClosePacket"/> class.
        /// </summary>
        /// <param name="containerId">The id of the container being closed.</param>
        public ContainerClosePacket(byte containerId)
        {
            this.ContainerId = containerId;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public OutgoingPacketType PacketType => OutgoingPacketType.ContainerClose;

        /// <summary>
        /// Gets the id of the container, as seen by the target player.
        /// </summary>
        public byte ContainerId { get; }
    }
}
