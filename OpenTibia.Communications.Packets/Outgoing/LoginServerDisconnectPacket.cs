// -----------------------------------------------------------------
// <copyright file="LoginServerDisconnectPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents an outgoing login server disconnection packet.
    /// </summary>
    public sealed class LoginServerDisconnectPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginServerDisconnectPacket"/> class.
        /// </summary>
        /// <param name="reason">Optional. The reason given for the disconnection, if any.</param>
        public LoginServerDisconnectPacket(string reason = "")
        {
            this.Reason = reason;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingManagementPacketType.Disconnect;

        /// <summary>
        /// Gets the reason given for the disconnection, if any.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteLoginServerDisconnectPacket(this);
        }
    }
}
