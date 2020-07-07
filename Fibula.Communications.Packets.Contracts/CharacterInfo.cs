// -----------------------------------------------------------------
// <copyright file="CharacterInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Contracts.Abstractions
{
    /// <summary>
    /// Provides the model for character information.
    /// </summary>
    public sealed class CharacterInfo
    {
        /// <summary>
        /// Gets or sets the name of the character.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the world where this character lives.
        /// </summary>
        public string World { get; set; }

        /// <summary>
        /// Gets or sets the IP address bytes that the client must use to connect if loging in with this character.
        /// </summary>
        public byte[] Ip { get; set; }

        /// <summary>
        /// Gets or sets the port that the client must use to connect if loging in with this character.
        /// </summary>
        public ushort Port { get; set; }
    }
}
