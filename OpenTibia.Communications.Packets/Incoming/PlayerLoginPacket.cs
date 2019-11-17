// -----------------------------------------------------------------
// <copyright file="PlayerLoginPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a player login packet.
    /// </summary>
    public sealed class PlayerLoginPacket : IIncomingPacket, IPlayerLoginInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLoginPacket"/> class.
        /// </summary>
        /// <param name="xteaKey">The XTEA key to use in all further communications with this client.</param>
        /// <param name="operatingSystem">The operating system of the player's client.</param>
        /// <param name="version">The version of the player's client.</param>
        /// <param name="isGamemaster">A value indicating whether this player is a gamemaster.</param>
        /// <param name="accountNumber">The account number of this player.</param>
        /// <param name="characterName">The character name.</param>
        /// <param name="password">The password used.</param>
        public PlayerLoginPacket(uint[] xteaKey, ushort operatingSystem, ushort version, bool isGamemaster, uint accountNumber, string characterName, string password)
        {
            this.XteaKey = xteaKey;

            this.Os = operatingSystem;
            this.Version = version;

            this.IsGm = isGamemaster;

            this.AccountNumber = accountNumber;
            this.CharacterName = characterName;
            this.Password = password;
        }

        /// <summary>
        /// Gets the operating system of the player's client.
        /// </summary>
        public ushort Os { get; }

        /// <summary>
        /// Gets the version of the player's client.
        /// </summary>
        public ushort Version { get; }

        /// <summary>
        /// Gets the XTEA key to use in all further communications with this client.
        /// </summary>
        public uint[] XteaKey { get; }

        /// <summary>
        /// Gets a value indicating whether this character is a gamemaster.
        /// </summary>
        public bool IsGm { get; }

        /// <summary>
        /// Gets the account number of the character.
        /// </summary>
        public uint AccountNumber { get; }

        /// <summary>
        /// Gets the character's name.
        /// </summary>
        public string CharacterName { get; }

        /// <summary>
        /// Gets the password used to log in.
        /// </summary>
        public string Password { get; }
    }
}
