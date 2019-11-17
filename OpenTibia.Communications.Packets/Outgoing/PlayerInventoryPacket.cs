// <copyright file="PlayerInventoryPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    public class PlayerInventoryPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerInventoryPacket"/> class.
        /// </summary>
        /// <param name="player"></param>
        public PlayerInventoryPacket(IPlayer player)
        {
            this.Player = player;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.InventoryItem;

        public IPlayer Player { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WritePlayerInventoryPacket(this);
        }
    }
}
