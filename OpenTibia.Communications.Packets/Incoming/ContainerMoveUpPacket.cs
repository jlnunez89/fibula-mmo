// <copyright file="ContainerMoveUpPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a move up container request packet.
    /// </summary>
    public class ContainerMoveUpPacket : IIncomingPacket, IContainerInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerMoveUpPacket"/> class.
        /// </summary>
        /// <param name="containerId">The id of the container.</param>
        public ContainerMoveUpPacket(byte containerId)
        {
            this.ContainerId = containerId;
        }

        /// <summary>
        /// Gets the id of the container.
        /// </summary>
        public byte ContainerId { get; }
    }
}
