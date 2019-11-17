// <copyright file="ContainerOpenPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using System.Collections.Generic;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    public class ContainerOpenPacket : IOutgoingPacket
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

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteContainerOpenPacket(this);
        }
    }
}