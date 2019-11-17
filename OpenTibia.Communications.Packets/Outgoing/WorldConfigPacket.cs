// <copyright file="WorldConfigPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    public class WorldConfigPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorldConfigPacket"/> class.
        /// </summary>
        /// <param name="dailyResetHour"></param>
        /// <param name="ipAddressBytes"></param>
        /// <param name="maximumRookgardians"></param>
        /// <param name="maximumTotalPlayers"></param>
        /// <param name="port"></param>
        /// <param name="premiumMainlandBuffer"></param>
        /// <param name="premiumRookgardiansBuffer"></param>
        /// <param name="worldType"></param>
        public WorldConfigPacket(byte dailyResetHour, byte[] ipAddressBytes, ushort maximumRookgardians, ushort maximumTotalPlayers, ushort port, ushort premiumMainlandBuffer, ushort premiumRookgardiansBuffer, byte worldType)
        {
            this.DailyResetHour = dailyResetHour;
            this.IpAddressBytes = ipAddressBytes;
            this.MaximumRookgardians = maximumRookgardians;
            this.MaximumTotalPlayers = maximumTotalPlayers;
            this.Port = port;
            this.PremiumMainlandBuffer = premiumMainlandBuffer;
            this.PremiumRookgardiansBuffer = premiumRookgardiansBuffer;
            this.WorldType = worldType;
        }

        public byte PacketType => (byte)OutgoingManagementPacketType.NoError;

        public byte DailyResetHour { get; }

        public byte[] IpAddressBytes { get; }

        public ushort MaximumRookgardians { get; }

        public ushort MaximumTotalPlayers { get; }

        public ushort Port { get; }

        public ushort PremiumMainlandBuffer { get; }

        public ushort PremiumRookgardiansBuffer { get; }

        public byte WorldType { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteWorldConfigPacket(this);
        }
    }
}