// <copyright file="InventoryClearSlotPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Enumerations;

    public class InventoryClearSlotPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryClearSlotPacket"/> class.
        /// </summary>
        /// <param name="slot"></param>
        public InventoryClearSlotPacket(Slot slot)
        {
            this.Slot = slot;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.InventoryEmpty;

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
