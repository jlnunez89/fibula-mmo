// <copyright file="ContainerRemoveItemPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    public class ContainerRemoveItemPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerRemoveItemPacket"/> class.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="containerId"></param>
        public ContainerRemoveItemPacket(byte index, byte containerId)
        {
            this.Index = index;
            this.ContainerId = containerId;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.ContainerRemoveItem;

        public byte Index { get; }

        public byte ContainerId { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteContainerRemoveItemPacket(this);
        }
    }
}