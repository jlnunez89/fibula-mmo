// -----------------------------------------------------------------
// <copyright file="IOutboundPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Abstractions
{
    /// <summary>
    /// Interface for all outbound packets.
    /// </summary>
    public interface IOutboundPacket
    {
        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        byte PacketType { get; }
    }
}
