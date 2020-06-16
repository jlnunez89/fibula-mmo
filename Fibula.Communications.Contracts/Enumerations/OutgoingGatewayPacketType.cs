// -----------------------------------------------------------------
// <copyright file="OutgoingGatewayPacketType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates de different outgoing gateway server packet types.
    /// </summary>
    public enum OutgoingGatewayPacketType : byte
    {
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
