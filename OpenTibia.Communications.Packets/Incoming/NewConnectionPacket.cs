// -----------------------------------------------------------------
// <copyright file="NewConnectionPacket.cs" company="2Dudes">
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
    /// Class that represents a new connection request packet.
    /// </summary>
    public sealed class NewConnectionPacket : IIncomingPacket, INewConnectionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewConnectionPacket"/> class.
        /// </summary>
        /// <param name="operatingSystem">The operating system.</param>
        /// <param name="version">The version of the client.</param>
        public NewConnectionPacket(ushort operatingSystem, ushort version)
        {
            this.Os = operatingSystem;
            this.Version = version;
        }

        /// <summary>
        /// Gets the operating system of the client requesting connection.
        /// </summary>
        public ushort Os { get; }

        /// <summary>
        /// Gets the version of the client requesting connection.
        /// </summary>
        public ushort Version { get; }
    }
}
