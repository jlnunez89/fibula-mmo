// -----------------------------------------------------------------
// <copyright file="PlayerInventoryClearSlotPacket.cs" company="2Dudes">
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
    using Fibula.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a player's clear inventory slot packet.
    /// </summary>
    public class PlayerInventoryClearSlotPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerInventoryClearSlotPacket"/> class.
        /// </summary>
        /// <param name="slot">The slot that this packet is about.</param>
        public PlayerInventoryClearSlotPacket(Slot slot)
        {
            this.Slot = slot;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.InventoryEmpty;

        /// <summary>
        /// Gets the slot.
        /// </summary>
        public Slot Slot { get; }
    }
}
