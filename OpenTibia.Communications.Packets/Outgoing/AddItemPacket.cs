// <copyright file="AddItemPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents a packet with information about an item that was added to the game.
    /// </summary>
    public class AddItemPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddItemPacket"/> class.
        /// </summary>
        /// <param name="location">The location of the item added.</param>
        /// <param name="item">The item added.</param>
        public AddItemPacket(Location location, IItem item)
        {
            this.Location = location;
            this.Item = item;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.AddAtStackpos;

        /// <summary>
        /// Gets the location of the item added.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets a reference to the added item.
        /// </summary>
        public IItem Item { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteAddItemPacket(this);
        }
    }
}
