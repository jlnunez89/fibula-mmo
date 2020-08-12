// -----------------------------------------------------------------
// <copyright file="ContainerAddItemPacket.cs" company="2Dudes">
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
    using Fibula.Items.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a packet for an item added to a container.
    /// </summary>
    public class ContainerAddItemPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerAddItemPacket"/> class.
        /// </summary>
        /// <param name="containerId">The id of the container.</param>
        /// <param name="item">The item being added.</param>
        public ContainerAddItemPacket(byte containerId, IItem item)
        {
            this.ContainerId = containerId;
            this.Item = item;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.ContainerAddItem;

        /// <summary>
        /// Gets the id of the container, as seen by the target player.
        /// </summary>
        public byte ContainerId { get; }

        /// <summary>
        /// Gets the item being added.
        /// </summary>
        public IItem Item { get; }
    }
}
