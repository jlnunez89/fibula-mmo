// <copyright file="BanismentResultPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    public class BanismentResultPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BanismentResultPacket"/> class.
        /// </summary>
        /// <param name="banDays">The number of days banished.</param>
        /// <param name="banishedUntil">The unix date time until which the banishment lasts.</param>
        public BanismentResultPacket(byte banDays, uint banishedUntil)
        {
            this.BanDays = banDays;
            this.BanishedUntil = banishedUntil;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingManagementPacketType.NoError;

        /// <summary>
        /// Gets the number of days that the banishment lasts for.
        /// </summary>
        public byte BanDays { get; }

        /// <summary>
        /// Gets the unix date time until which the banishment lasts.
        /// </summary>
        public uint BanishedUntil { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteBanismentResultPacket(this);
        }
    }
}