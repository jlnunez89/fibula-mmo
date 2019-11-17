// <copyright file="AuthenticationResultPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a packet with information about an authentication result.
    /// </summary>
    public class AuthenticationResultPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationResultPacket"/> class.
        /// </summary>
        /// <param name="hadError">A value indicating wether there was an error during authentication.</param>
        public AuthenticationResultPacket(bool hadError)
        {
            this.HadError = hadError;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingManagementPacketType.NoError;

        /// <summary>
        /// Gets a value indicating whether there was an error during authentication.
        /// </summary>
        public bool HadError { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteAuthenticationResultPacket(this);
        }
    }
}
