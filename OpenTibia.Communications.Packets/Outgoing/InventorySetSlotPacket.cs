// <copyright file="InventorySetSlotPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    public class InventorySetSlotPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventorySetSlotPacket"/> class.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="item"></param>
        public InventorySetSlotPacket(Slot slot, IItem item)
        {
            this.Slot = slot;
            this.Item = item;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.InventoryItem;

        public Slot Slot { get; }

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
