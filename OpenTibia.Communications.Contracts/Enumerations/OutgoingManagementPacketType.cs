// -----------------------------------------------------------------
// <copyright file="OutgoingManagementPacketType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates de different outgoing management packet types.
    /// </summary>
    public enum OutgoingManagementPacketType : byte
    {
        /// <summary>
        /// A type to use when there is no error.
        /// </summary>
        NoError = 0x00,

        /// <summary>
        /// A type to use when there is an error.
        /// </summary>
        Error = 0x01,

        /// <summary>
        /// Disconnection from the server.
        /// </summary>
        Disconnect = 0x0A,

        /// <summary>
        /// The message of the day.
        /// </summary>
        MessageOfTheDay = 0x14,

        /// <summary>
        /// A character list.
        /// </summary>
        CharacterList = 0x64,
    }
}
