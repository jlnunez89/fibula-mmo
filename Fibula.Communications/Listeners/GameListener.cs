// -----------------------------------------------------------------
// <copyright file="GameListener.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Listeners
{
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Security.Contracts;
    using Microsoft.Extensions.Options;
    using Serilog;

    /// <summary>
    /// Class that extends the standard <see cref="BaseListener"/> for a game listener.
    /// </summary>
    /// <typeparam name="TConnectionFactory">The type of connection factory to use in the listener.</typeparam>
    public class GameListener<TConnectionFactory> : BaseListener
        where TConnectionFactory : class, ISocketConnectionFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameListener{T}"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="options">The options for this listener.</param>
        /// <param name="connectionFactory">A reference to the connection factory in play here.</param>
        /// <param name="dosDefender">A reference to the DoS defender service implementation.</param>
        public GameListener(
            ILogger logger,
            IOptions<GameListenerOptions> options,
            TConnectionFactory connectionFactory,
            IDoSDefender dosDefender)
            : base(logger, options?.Value, connectionFactory, dosDefender)
        {
        }
    }
}
