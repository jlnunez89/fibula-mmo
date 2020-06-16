// -----------------------------------------------------------------
// <copyright file="TurnSouthHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Protocol.V772.RequestHandlers
{
    using Fibula.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents the player turning south handler.
    /// </summary>
    public class TurnSouthHandler : TurnToDirectionHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TurnSouthHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        public TurnSouthHandler(ILogger logger, IGameContext gameContext)
            : base(logger, gameContext, Direction.South)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForRequestType => (byte)GameRequestType.TurnSouth;
    }
}