// -----------------------------------------------------------------
// <copyright file="AutoMovePacket.cs" company="2Dudes">
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
    using System;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an auto movement packet.
    /// </summary>
    public class AutoMovePacket : IAutoMovementInfo
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
