// <copyright file="AutoMovePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using System;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents an auto movement packet.
    /// </summary>
    public class AutoMovePacket : IIncomingPacket, IAutoMovementInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMovePacket"/> class.
        /// </summary>
        /// <param name="directions">The directions to follow, in order.</param>
        public AutoMovePacket(params Direction[] directions)
        {
            if (directions == null || directions.Length == 0)
            {
                throw new ArgumentException($"{nameof(directions)} must contain at least one element.", nameof(directions));
            }

            this.Directions = directions;
        }

        /// <summary>
        /// Gets the movement directions.
        /// </summary>
        public Direction[] Directions { get; }
    }
}
