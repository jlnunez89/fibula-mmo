﻿// -----------------------------------------------------------------
// <copyright file="AutoMovePacket.cs" company="2Dudes">
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
    using System;
    using Fibula.Communications.Contracts.Abstractions.RequestInfo;
    using Fibula.Server.Contracts.Enumerations;

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
