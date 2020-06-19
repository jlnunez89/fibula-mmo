// -----------------------------------------------------------------
// <copyright file="GameLogOutPacket.cs" company="2Dudes">
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
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a logout packet routed to the game server.
    /// </summary>
    public sealed class GameLogOutPacket : IActionWithoutContentInfo
    {
        /// <summary>
        /// Gets the action to do.
        /// </summary>
        public IncomingGamePacketType Action => IncomingGamePacketType.LogOut;
    }
}
