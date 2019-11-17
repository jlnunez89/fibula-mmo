// -----------------------------------------------------------------
// <copyright file="CharacterListItem.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Messaging.Packets
{
    using System.Net;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an item on the Character list.
    /// </summary>
    public class CharacterListItem : ICharacterListItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterListItem"/> class.
        /// </summary>
        /// <param name="name">The character name to use.</param>
        /// <param name="ip">The public IP address for the world's connection.</param>
        /// <param name="port">The public port for the world's connection.</param>
        /// <param name="world">The world where this character lives.</param>
        public CharacterListItem(string name, IPAddress ip, ushort port, string world)
        {
            name.ThrowIfNullOrWhiteSpace(nameof(name));
            ip.ThrowIfNull(nameof(ip));
            port.ThrowIfDefaultValue(nameof(port));
            world.ThrowIfNullOrWhiteSpace(nameof(world));

            this.Name = name;
            this.World = world;
            this.Ip = ip.GetAddressBytes();
            this.Port = port;
        }

        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the name of the world where this character lives.
        /// </summary>
        public string World { get; }

        /// <summary>
        /// Gets the IP address bytes that the client must use to connect if loging in with this character.
        /// </summary>
        public byte[] Ip { get; }

        /// <summary>
        /// Gets the port that the client must use to connect if loging in with this character.
        /// </summary>
        public ushort Port { get; }
    }
}