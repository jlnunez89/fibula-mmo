// -----------------------------------------------------------------
// <copyright file="PlayerWalkSouthwestHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Handlers.Game
{
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents the player walking southwest handler.
    /// </summary>
    public class PlayerWalkSouthwestHandler : PlayerWalkOnDemandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerWalkSouthwestHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        public PlayerWalkSouthwestHandler(IGame gameInstance, ICreatureFinder creatureFinder)
            : base(gameInstance, creatureFinder, Direction.SouthWest)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.WalkSouthwest;
    }
}