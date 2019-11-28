// -----------------------------------------------------------------
// <copyright file="ICharacterLoginInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    /// <summary>
    /// Interface for character login information.
    /// </summary>
    public interface ICharacterLoginInfo
    {
        /// <summary>
        /// Gets the operating system of the character's client.
        /// </summary>
        ushort Os { get; }

        /// <summary>
        /// Gets the version of the character's client.
        /// </summary>
        ushort Version { get; }

        /// <summary>
        /// Gets the XTEA key to use in all further communications with this client.
        /// </summary>
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