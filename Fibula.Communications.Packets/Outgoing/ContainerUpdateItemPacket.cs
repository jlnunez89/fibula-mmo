// -----------------------------------------------------------------
// <copyright file="ContainerUpdateItemPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Outgoing
{
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Items.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a packet for an item being updated within a container.
    /// </summary>
    public class ContainerUpdateItemPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerUpdateItemPacket"/> class.
        /// </summary>
        /// <param name="index">The index within the container, at which the item being updated is.</param>
        /// <param name="containerId">The id of the container.</param>
        /// <param name="item">The item being updated.</param>
        public ContainerUpdateItemPacket(byte index, byte containerId, IItem item)
        {
            this.Index = index;
            this.ContainerId = containerId;
            this.Item = item;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.ContainerUpdateItem;

        /// <summary>
        /// Gets the index within the container, at which the item being updated is.
        /// </summary>
        public byte Index { get; }

        /// <summary>
        /// Gets the id of the container.
        /// </summary>
        public byte ContainerId { get; }

        /// <summary>
        /// Gets the item being updated.
        /// </summary>
        public IItem Item { get; }
    }
}