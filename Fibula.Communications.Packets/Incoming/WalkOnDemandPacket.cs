// -----------------------------------------------------------------
// <copyright file="WalkOnDemandPacket.cs" company="2Dudes">
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
    using Fibula.Communications.Packets.Contracts.Abstractions;
    using Fibula.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a login packet routed to the game server.
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
