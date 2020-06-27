// -----------------------------------------------------------------
// <copyright file="PlayerLoginPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Outgoing
{
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a player login packet.
    /// </summary>
    public class PlayerLoginPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLoginPacket"/> class.
        /// </summary>
        /// <param name="creatureId">The id of the creature.</param>
        /// <param name="player">A reference to the player.</param>
        public PlayerLoginPacket(uint creatureId, IPlayer player)
        {
            this.CreatureId = creatureId;
            this.Player = player;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.PlayerLogin;

        /// <summary>
        /// Gets the id of this creature.
        /// </summary>
        public uint CreatureId { get; }

        /// <summary>
        /// Gets the graphics speed.
        /// Should always be 50 apparently.
        /// </summary>
        public byte GraphicsSpeed => 0x32;

        /// <summary>
        /// Gets a value indicating whether the player can report bugs.
        /// </summary>
        public byte CanReportBugs => 0x00;

        /// <summary>
        /// Gets a reference to the player.
        /// </summary>
        public IPlayer Player { get; }
    }
}
