// -----------------------------------------------------------------
// <copyright file="AutoMoveCancelPacket.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Incoming
{
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an auto movement cancellation packet routed to the game server.
    /// </summary>
    public sealed class AutoMoveCancelPacket : IIncomingPacket, IActionWithoutContentInfo
    {
        /// <summary>
        /// Gets the action to do.
        /// </summary>
        public IncomingPacketType Action => IncomingPacketType.AutoMoveCancel;
    }
}
