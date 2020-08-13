// -----------------------------------------------------------------
// <copyright file="GatewayServerDisconnectPacket.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
        public OutgoingPacketType PacketType => OutgoingPacketType.GatewayDisconnect;

        /// <summary>
        /// Gets the reason given for the disconnection, if any.
        /// </summary>
        public string Reason { get; }
    }
}
