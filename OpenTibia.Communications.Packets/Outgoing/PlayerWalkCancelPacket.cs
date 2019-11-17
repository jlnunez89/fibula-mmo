// <copyright file="PlayerWalkCancelPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Enumerations;

    public class PlayerWalkCancelPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerWalkCancelPacket"/> class.
        /// </summary>
        /// <param name="direction"></param>
        public PlayerWalkCancelPacket(Direction direction)
        {
            this.Direction = direction;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.PlayerWalkCancel;

        public Direction Direction { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WritePlayerWalkCancelPacket(this);
        }
    }
}
