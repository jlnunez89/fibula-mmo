// -----------------------------------------------------------------
// <copyright file="PlayerInventorySetSlotPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a player's filled inventory slot packet.
    /// </summary>
    public class PlayerInventorySetSlotPacket : IOutgoingPacket
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

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteInventorySetSlotPacket(this);
        }
    }
}
