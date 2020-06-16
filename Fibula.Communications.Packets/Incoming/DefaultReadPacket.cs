// -----------------------------------------------------------------
// <copyright file="DefaultReadPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Incoming
{
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents the default packet.
    /// </summary>
    public sealed class DefaultReadPacket : IBytesInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultReadPacket"/> class.
        /// </summary>
        /// <param name="bytes">The bytes that represent the packet.</param>
        public DefaultReadPacket(params byte[] bytes)
        {
            bytes.ThrowIfNull(nameof(bytes));

            this.Bytes = bytes;
        }

        /// <summary>
        /// Gets the collection of bytes that represent the packet.
        /// </summary>
        public byte[] Bytes { get; }
    }
}
