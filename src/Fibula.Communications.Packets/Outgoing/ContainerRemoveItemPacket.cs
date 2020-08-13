// -----------------------------------------------------------------
// <copyright file="ContainerRemoveItemPacket.cs" company="2Dudes">
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
    /// Class that represents a packet for an item being removed to a container.
    /// </summary>
    public class ContainerRemoveItemPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerRemoveItemPacket"/> class.
        /// </summary>
        /// <param name="index">The index within the container, at which the item is being removed.</param>
        /// <param name="containerId">The id of the container.</param>
        public ContainerRemoveItemPacket(byte index, byte containerId)
        {
            this.Index = index;
            this.ContainerId = containerId;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public OutgoingPacketType PacketType => OutgoingPacketType.ContainerRemoveItem;

        /// <summary>
        /// Gets the index within the container, at which the item is being removed.
        /// </summary>
        public byte Index { get; }

        /// <summary>
        /// Gets the id of the container.
        /// </summary>
        public byte ContainerId { get; }
    }
}
