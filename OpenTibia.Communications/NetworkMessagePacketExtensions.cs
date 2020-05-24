// -----------------------------------------------------------------
// <copyright file="NetworkMessagePacketExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications
{
    using OpenTibia.Communications.Contracts.Abstractions;

    /// <summary>
    /// Static class that defines extension methods for <see cref="INetworkMessage"/>, which allow it to
    /// read and write the packets contained in this assembly.
    /// </summary>
    public static class NetworkMessagePacketExtensions
    {
        /// <summary>
        /// Reads default information sent in the message.
        /// </summary>
        /// <param name="message">The mesage to read the information from.</param>
        /// <returns>The default formatted information.</returns>
        public static IDefaultInfo ReadDefaultInfo(this INetworkMessage message)
        {
            return new DefaultReadPacket(message.GetBytes(message.Length - message.Cursor));
        }
    }
}
