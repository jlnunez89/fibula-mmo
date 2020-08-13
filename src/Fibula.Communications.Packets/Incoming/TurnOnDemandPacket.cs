// -----------------------------------------------------------------
// <copyright file="TurnOnDemandPacket.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a turn packet routed to the game server.
    /// </summary>
    public sealed class TurnOnDemandPacket : IIncomingPacket, ITurnOnDemandInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TurnOnDemandPacket"/> class.
        /// </summary>
        /// <param name="direction">The direction to turn to.</param>
        public TurnOnDemandPacket(Direction direction)
        {
            this.Direction = direction;
        }

        /// <summary>
        /// Gets the direction to walk to.
        /// </summary>
        public Direction Direction { get; }
    }
}
