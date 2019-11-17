// -----------------------------------------------------------------
// <copyright file="AuthenticationPacket.cs" company="2Dudes">
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
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an authentication packet.
    /// </summary>
    public sealed class AuthenticationPacket : IIncomingPacket, IAuthenticationInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationPacket"/> class.
        /// </summary>
        /// <param name="password">The password to authenticate with.</param>
        /// <param name="worldName">The world name to authenticate to.</param>
        public AuthenticationPacket(string password, string worldName)
        {
            password.ThrowIfNullOrWhiteSpace(nameof(password));
            worldName.ThrowIfNullOrWhiteSpace(nameof(worldName));

            this.Password = password;
            this.WorldName = worldName;
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Gets the world name.
        /// </summary>
        public string WorldName { get; }
    }
}
