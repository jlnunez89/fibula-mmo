// -----------------------------------------------------------------
// <copyright file="IncomingGatewayPacketType.cs" company="2Dudes">
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