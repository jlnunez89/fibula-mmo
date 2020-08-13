// -----------------------------------------------------------------
// <copyright file="GameServerDisconnectPacket.cs" company="2Dudes">
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
    /// Class that represents a game server disconnect packet.
    /// </summary>
    public class GameServerDisconnectPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameServerDisconnectPacket"/> class.
        /// </summary>
        /// <param name="reason">Optional. A reason for the disconnection.</param>
        public GameServerDisconnectPacket(string reason = "")
        {
            this.Reason = reason;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public OutgoingPacketType PacketType => OutgoingPacketType.GameDisconnect;

        /// <summary>
        /// Gets the reason given for the disconnection, if any.
        /// </summary>
        public string Reason { get; }
    }
}
