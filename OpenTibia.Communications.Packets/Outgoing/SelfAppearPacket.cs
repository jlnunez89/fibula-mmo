// -----------------------------------------------------------------
// <copyright file="SelfAppearPacket.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a self appear packet.
    /// </summary>
    public class SelfAppearPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfAppearPacket"/> class.
        /// </summary>
        /// <param name="creatureId">The id of the creature.</param>
        /// <param name="isLogin">A value indicating whether this packet is being sent because of a login.</param>
        /// <param name="player">A reference to the player.</param>
        public SelfAppearPacket(uint creatureId, bool isLogin, IPlayer player)
        {
            this.CreatureId = creatureId;
            this.IsLogin = isLogin;
            this.Player = player;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.SelfAppear;

        /// <summary>
        /// Gets the id of this creature.
        /// </summary>
        public uint CreatureId { get; }

        /// <summary>
        /// Gets the graphics speed.
        /// Should always be 32 apparently.
        /// </summary>
        public byte GraphicsSpeed => 0x32;

        /// <summary>
        /// Gets a value indicating whether the player can report bugs.
        /// </summary>
        public byte CanReportBugs => 0x00;

        /// <summary>
        /// Gets a value indicating whether this packet is the first packet being sent to the client.
        /// </summary>
        public bool IsLogin { get; }

        /// <summary>
        /// Gets a reference to the player.
        /// </summary>
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
