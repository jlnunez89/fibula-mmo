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
    using Fibula.Creatures.Contracts.Abstractions;
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
        public byte PacketType => (byte)GameResponseType.InventoryEmpty;

        /// <summary>
        /// Gets the slot.
        /// </summary>
        public Slot Slot { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteInventoryClearSlotPacket(this);
        }
    }
}
