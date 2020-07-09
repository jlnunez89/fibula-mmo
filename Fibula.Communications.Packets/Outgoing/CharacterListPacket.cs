// -----------------------------------------------------------------
// <copyright file="CharacterListPacket.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Outgoing
{
    using System.Collections.Generic;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an outgoing character list packet.
    /// </summary>
    public sealed class CharacterListPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterListPacket"/> class.
        /// </summary>
        /// <param name="characters">The list of characters in the account.</param>
        /// <param name="premDays">The premium days left on the account.</param>
        public CharacterListPacket(IEnumerable<CharacterInfo> characters, ushort premDays)
        {
            characters.ThrowIfNull(nameof(characters));

            this.Characters = characters;
            this.PremiumDaysLeft = premDays;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGatewayPacketType.CharacterList;

        /// <summary>
        /// Gets the list of characters in the account.
        /// </summary>
        public IEnumerable<CharacterInfo> Characters { get; }

        /// <summary>
        /// Gets the premium days left on the account.
        /// </summary>
        public ushort PremiumDaysLeft { get; }
    }
}
