// <copyright file="ServerStatusPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    public class ServerStatusPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerStatusPacket"/> class.
        /// </summary>
        /// <param name="data"></param>
        public ServerStatusPacket(string data)
        {
            this.Data = data;
        }

        public byte PacketType => (byte)OutgoingManagementPacketType.NoError;

        public string Data { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteServerStatusPacket(this);
        }
    }
}
