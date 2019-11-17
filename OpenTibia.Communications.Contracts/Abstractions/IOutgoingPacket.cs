// -----------------------------------------------------------------
// <copyright file="IOutgoingPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts.Abstractions
{
    /// <summary>
    /// Interface for all outgoing packets.
    /// </summary>
    public interface IOutgoingPacket
    {
        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        byte PacketType { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        void WriteToMessage(INetworkMessage message);
    }
}
