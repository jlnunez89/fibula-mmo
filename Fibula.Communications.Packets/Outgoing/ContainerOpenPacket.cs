// -----------------------------------------------------------------
// <copyright file="ContainerOpenPacket.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Items.Contracts.Abstractions;

    public class ContainerOpenPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerOpenPacket"/> class.
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="clientItemId"></param>
        /// <param name="name"></param>
        /// <param name="volume"></param>
        /// <param name="hasParent"></param>
        /// <param name="contents"></param>
        public ContainerOpenPacket(byte containerId, ushort clientItemId, string name, byte volume, bool hasParent, IList<IItem> contents)
        {
            this.ContainerId = containerId;
            this.ClientItemId = clientItemId;
            this.Name = name;
            this.Volume = volume;
            this.HasParent = hasParent;
            this.Contents = contents;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.ContainerOpen;

        public byte ContainerId { get; }

        public ushort ClientItemId { get; }

        public string Name { get; }

        public byte Volume { get; }

        public bool HasParent { get; }

        public IList<IItem> Contents { get; }
    }
}