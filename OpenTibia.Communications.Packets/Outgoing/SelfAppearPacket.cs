// <copyright file="SelfAppearPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    public class SelfAppearPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfAppearPacket"/> class.
        /// </summary>
        /// <param name="creatureId"></param>
        /// <param name="isLogin"></param>
        /// <param name="player"></param>
        public SelfAppearPacket(uint creatureId, bool isLogin, IPlayer player)
        {
            this.CreatureId = creatureId;
            this.IsLogin = isLogin;
            this.Player = player;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.SelfAppear;

        public uint CreatureId { get; }

        public byte GraphicsSpeed => 0x32; // Should always be 32 apparently...

        public byte CanReportBugs => 0x00;

        public bool IsLogin { get; }

        public IPlayer Player { get; }

        // public HashSet<string> Privileges { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteSelfAppearPacket(this);
        }
    }
}
