// <copyright file="GameHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a handler that exposes a reference to the game instance, for convenience.
    /// </summary>
    public abstract class GameHandler : BaseHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameHandler"/> class.
        /// </summary>
        /// <param name="gameInstance">A reference to the game instance.</param>
        protected GameHandler(IGame gameInstance)
            : base()
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            this.Game = gameInstance;
        }

        /// <summary>
        /// Gets the reference to the game instance.
        /// </summary>
        protected IGame Game { get; }
    }
}