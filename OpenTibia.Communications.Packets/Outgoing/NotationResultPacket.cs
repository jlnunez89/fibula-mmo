// <copyright file="NotationResultPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    public class NotationResultPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotationResultPacket"/> class.
        /// </summary>
        /// <param name="gamemasterId"></param>
        public NotationResultPacket(uint gamemasterId)
        {
            this.GamemasterId = gamemasterId;
        }

        public byte PacketType => (byte)OutgoingManagementPacketType.NoError;

        public uint GamemasterId { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteNotationResultPacket(this);
        }
    }
}