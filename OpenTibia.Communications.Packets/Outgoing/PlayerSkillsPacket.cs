// <copyright file="PlayerSkillsPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    public class PlayerSkillsPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSkillsPacket"/> class.
        /// </summary>
        /// <param name="player"></param>
        public PlayerSkillsPacket(IPlayer player)
        {
            this.Player = player;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.PlayerSkills;

        public IPlayer Player { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WritePlayerSkillsPacket(this);
        }
    }
}
