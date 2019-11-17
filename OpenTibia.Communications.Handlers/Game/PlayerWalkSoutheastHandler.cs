// -----------------------------------------------------------------
// <copyright file="PlayerWalkSoutheastHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Handlers.Game
{
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    public class PlayerWalkSoutheastHandler : PlayerWalkOnDemandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerWalkSoutheastHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public PlayerWalkSoutheastHandler(IGame gameInstance)
            : base(gameInstance, Direction.SouthEast)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.WalkSoutheast;
    }
}