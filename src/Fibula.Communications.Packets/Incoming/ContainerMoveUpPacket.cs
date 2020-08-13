// -----------------------------------------------------------------
// <copyright file="ContainerMoveUpPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Incoming
{
    using Fibula.Communications.Packets.Contracts.Abstractions;

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
