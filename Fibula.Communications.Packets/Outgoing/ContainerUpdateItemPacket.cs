// -----------------------------------------------------------------
// <copyright file="ContainerUpdateItemPacket.cs" company="2Dudes">
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
    using Fibula.Items.Contracts.Abstractions;

    public class ContainerUpdateItemPacket : IOutboundPacket
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
    }
}