// <copyright file="ContainerUpdateItemPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    public class ContainerUpdateItemPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerUpdateItemPacket"/> class.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="containerId"></param>
        /// <param name="item"></param>
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

        public byte Index { get; }

        public byte ContainerId { get; }

        public IItem Item { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteContainerUpdateItemPacket(this);
        }
    }
}