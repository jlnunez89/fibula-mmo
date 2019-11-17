// <copyright file="CharacterDeathPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using System;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a character death packet.
    /// </summary>
    public class CharacterDeathPacket : IIncomingPacket, ICharacterDeathInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterDeathPacket"/> class.
        /// </summary>
        /// <param name="victimId">The id of the victim.</param>
        /// <param name="victimLevel">The level of the victim.</param>
        /// <param name="killerId">The id of the killer.</param>
        /// <param name="killerName">The name of the killer.</param>
        /// <param name="wasUnjustified">A value indicating whether the kill was unjustified.</param>
        /// <param name="timestamp">The date and time when the death took place.</param>
        public CharacterDeathPacket(
            uint victimId,
            ushort victimLevel,
            uint killerId,
            string killerName,
            bool wasUnjustified,
            DateTimeOffset timestamp)
        {
            this.VictimId = victimId;
            this.VictimLevel = victimLevel;
            this.KillerId = killerId;
            this.KillerName = killerName;
            this.Unjustified = wasUnjustified;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the victim character player id.
        /// </summary>
        public uint VictimId { get; }

        /// <summary>
        /// Gets the victim character level.
        /// </summary>
        public ushort VictimLevel { get; }

        /// <summary>
        /// Gets the killer player id, if available.
        /// </summary>
        public uint KillerId { get; }

        /// <summary>
        /// Gets the killer's name.
        /// </summary>
        public string KillerName { get; }

        /// <summary>
        /// Gets a value indicating whether the killing was unjustified.
        /// </summary>
        public bool Unjustified { get; }

        /// <summary>
        /// Gets the date and time of the death.
        /// </summary>
        public DateTimeOffset Timestamp { get; }
    }
}
