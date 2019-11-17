// -----------------------------------------------------------------
// <copyright file="CharacterListPacket.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an outgoing character list packet.
    /// </summary>
    public sealed class CharacterListPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterListPacket"/> class.
        /// </summary>
        /// <param name="characters">The list of characters in the account.</param>
        /// <param name="premDays">The premium days left on the account.</param>
        public CharacterListPacket(IEnumerable<ICharacterListItem> characters, ushort premDays)
        {
            this.Characters = characters;
            this.PremiumDaysLeft = premDays;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingManagementPacketType.CharacterList;

        /// <summary>
        /// Gets the list of characters in the account.
        /// </summary>
        public IEnumerable<ICharacterListItem> Characters { get; }

        /// <summary>
        /// Gets the premium days left on the account.
        /// </summary>
        public ushort PremiumDaysLeft { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteCharacterListPacket(this);
        }
    }
}
