// <copyright file="ContainerClosePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    public class ContainerClosePacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerClosePacket"/> class.
        /// </summary>
        /// <param name="containerId"></param>
        public ContainerClosePacket(byte containerId)
        {
            this.ContainerId = containerId;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.ContainerClose;

        public byte ContainerId { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteContainerClosePacket(this);
        }
    }
}