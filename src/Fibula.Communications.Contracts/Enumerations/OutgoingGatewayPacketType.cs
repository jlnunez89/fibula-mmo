// -----------------------------------------------------------------
// <copyright file="OutgoingGatewayPacketType.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
