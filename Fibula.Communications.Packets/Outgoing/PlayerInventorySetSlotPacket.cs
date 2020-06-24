// -----------------------------------------------------------------
// <copyright file="PlayerInventorySetSlotPacket.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Items.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a player's filled inventory slot packet.
    /// </summary>
    public class PlayerInventorySetSlotPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerInventorySetSlotPacket"/> class.
        /// </summary>
        /// <param name="slot">The slot that this packet is about.</param>
        /// <param name="item">The item that the slot contains.</param>
        public PlayerInventorySetSlotPacket(Slot slot, IItem item)
        {
            this.Slot = slot;
            this.Item = item;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.InventoryItem;

        /// <summary>
        /// Gets the slot.
        /// </summary>
        public Slot Slot { get; }

        /// <summary>
        /// Gets the item filling the slot.
        /// </summary>
        public IItem Item { get; }
    }
}
