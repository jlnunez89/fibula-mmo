// -----------------------------------------------------------------
// <copyright file="LoginServerDisconnectPacket.cs" company="2Dudes">
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

    /// <summary>
    /// Class that represents an outgoing login server disconnection packet.
    /// </summary>
    public sealed class GatewayServerDisconnectPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayServerDisconnectPacket"/> class.
        /// </summary>
        /// <param name="reason">Optional. The reason given for the disconnection, if any.</param>
        public GatewayServerDisconnectPacket(string reason = "")
        {
            this.Reason = reason;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGatewayPacketType.Disconnect;

        /// <summary>
        /// Gets the reason given for the disconnection, if any.
        /// </summary>
        public string Reason { get; }
    }
}
