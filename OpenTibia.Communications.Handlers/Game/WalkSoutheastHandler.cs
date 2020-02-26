﻿// -----------------------------------------------------------------
// <copyright file="WalkSoutheastHandler.cs" company="2Dudes">
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
    using Serilog;

    /// <summary>
    /// Class that represents the player walking southeast handler.
    /// </summary>
    public class WalkSoutheastHandler : WalkOnDemandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WalkSoutheastHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="operationFactory">A reference to the operation factory in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        public WalkSoutheastHandler(ILogger logger, IOperationFactory operationFactory, IGameContext gameContext)
            : base(logger, operationFactory, gameContext, Direction.SouthEast)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.WalkSoutheast;
    }
}