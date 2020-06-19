// -----------------------------------------------------------------
// <copyright file="ContainerRemoveItemPacket.cs" company="2Dudes">
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

    public class ContainerRemoveItemPacket : IOutboundPacket
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
    }
}