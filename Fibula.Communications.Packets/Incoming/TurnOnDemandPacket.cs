// -----------------------------------------------------------------
// <copyright file="TurnOnDemandPacket.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a turn packet routed to the game server.
    /// </summary>
    public sealed class TurnOnDemandPacket : ITurnOnDemandInfo
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
