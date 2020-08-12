// -----------------------------------------------------------------
// <copyright file="GameLogInPacket.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Incoming
{
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a login packet routed to the game server.
    /// </summary>
    public sealed class GameLogInPacket : IGameLogInInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogInPacket"/> class.
        /// </summary>
        /// <param name="xteaKey">The XTEA key to use in all further communications with this client.</param>
        /// <param name="operatingSystem">The operating system of the character's client.</param>
        /// <param name="version">The version of the character's client.</param>
        /// <param name="isGamemaster">A value indicating whether this character is a gamemaster.</param>
        /// <param name="accountNumber">The account number of this character.</param>
        /// <param name="characterName">The character name.</param>
        /// <param name="password">The password used.</param>
        public GameLogInPacket(uint[] xteaKey, ushort operatingSystem, ushort version, bool isGamemaster, uint accountNumber, string characterName, string password)
        {
            this.XteaKey = xteaKey;

            this.ClientOs = operatingSystem;
            this.ClientVersion = version;

            this.IsGamemaster = isGamemaster;

            this.AccountNumber = accountNumber;
            this.CharacterName = characterName;
            this.Password = password;
        }

        /// <summary>
        /// Gets the operating system of the character's client.
        /// </summary>
        public ushort ClientOs { get; }

        /// <summary>
        /// Gets the version of the character's client.
        /// </summary>
        public ushort ClientVersion { get; }

        /// <summary>
        /// Gets the XTEA key to use in all further communications with this client.
        /// </summary>
        public uint[] XteaKey { get; }

        /// <summary>
        /// Gets a value indicating whether this character is a gamemaster.
        /// </summary>
        public bool IsGamemaster { get; }

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
