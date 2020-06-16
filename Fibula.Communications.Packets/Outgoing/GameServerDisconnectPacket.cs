// -----------------------------------------------------------------
// <copyright file="GameServerDisconnectPacket.cs" company="2Dudes">
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
        public byte PacketType => (byte)OutgoingGamePacketType.Disconnect;

        /// <summary>
        /// Gets the reason given for the disconnection, if any.
        /// </summary>
        public string Reason { get; }
    }
}
