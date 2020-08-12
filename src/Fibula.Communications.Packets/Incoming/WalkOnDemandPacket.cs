// -----------------------------------------------------------------
// <copyright file="WalkOnDemandPacket.cs" company="2Dudes">
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
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a walk packet routed to the game server.
    /// </summary>
    public sealed class WalkOnDemandPacket : IWalkOnDemandInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WalkOnDemandPacket"/> class.
        /// </summary>
        /// <param name="direction">The direction to walk to.</param>
        public WalkOnDemandPacket(Direction direction)
        {
            this.Direction = direction;
        }

        /// <summary>
        /// Gets the direction to walk to.
        /// </summary>
        public Direction Direction { get; }
    }
}
