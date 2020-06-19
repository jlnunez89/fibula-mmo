// -----------------------------------------------------------------
// <copyright file="GameHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Mechanics.Handlers
{
    using Fibula.Common.Utilities;
    using Fibula.Server.Mechanics.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents a handler that exposes a reference to the game instance, for convenience.
    /// </summary>
    public abstract class GameHandler : BaseRequestHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="game">A reference to the game instance.</param>
        protected GameHandler(ILogger logger, IGame game)
            : base(logger)
        {
            game.ThrowIfNull(nameof(game));

            this.Game = game;
        }

        /// <summary>
        /// Gets the reference to the game instance.
        /// </summary>
        protected IGame Game { get; }
    }
}