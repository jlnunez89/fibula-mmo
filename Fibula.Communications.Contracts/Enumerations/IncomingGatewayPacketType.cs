// -----------------------------------------------------------------
// <copyright file="IncomingGatewayPacketType.cs" company="2Dudes">
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
    /// Enumerates the different incoming gateway server packet types.
    /// </summary>
    public enum IncomingGatewayPacketType : byte
    {
        /// <summary>
        /// A request to log into the game server.
        /// </summary>
        LogInRequest = 0x01,

        /// <summary>
        /// A request for information about the game server.
        /// </summary>
        ServerStatus = byte.MaxValue,
    }
}
