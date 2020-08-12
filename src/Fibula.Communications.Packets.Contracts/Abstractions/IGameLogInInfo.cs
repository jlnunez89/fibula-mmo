// -----------------------------------------------------------------
// <copyright file="IGameLogInInfo.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Contracts.Abstractions
{
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Interface for login information supplied on a game server login request.
    /// </summary>
    public interface IGameLogInInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the operating system of the character's client.
        /// </summary>
        ushort ClientOs { get; }

        /// <summary>
        /// Gets the version of the character's client.
        /// </summary>
        ushort ClientVersion { get; }

        /// <summary>
        /// Gets the XTEA key to use in all further communications with this client.
        /// </summary>
        // TODO: This is only relevant to TcpClients, and should be replaced with something more abstract.
        uint[] XteaKey { get; }

        /// <summary>
        /// Gets a value indicating whether this character is a gamemaster.
        /// </summary>
        bool IsGamemaster { get; }

        /// <summary>
        /// Gets the account number of the character.
        /// </summary>
        uint AccountNumber { get; }

        /// <summary>
        /// Gets the character's name.
        /// </summary>
        string CharacterName { get; }

        /// <summary>
        /// Gets the password used to log in.
        /// </summary>
        string Password { get; }
    }
}
