// -----------------------------------------------------------------
// <copyright file="LoginHandlerSelector.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Protocol772
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents a handler selector for incoming login requests.
    /// </summary>
    public class LoginHandlerSelector : IHandlerSelector
    {
        /// <summary>
        /// Stores a reference to the logger in use.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The known handlers to pick from.
        /// </summary>
        private readonly List<IHandler> handlersKnown;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginHandlerSelector"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="handlersRegistered">The collection of handlers registered in the configuration root of the service.</param>
        public LoginHandlerSelector(ILogger logger, IEnumerable<IHandler> handlersRegistered)
        {
            logger.ThrowIfNull(nameof(logger));
            handlersRegistered.ThrowIfNull(nameof(handlersRegistered));

            this.logger = logger.ForContext<GameHandlerSelector>();
            this.handlersKnown = handlersRegistered.ToList();
        }

        /// <summary>
        /// Returns the most appropriate handler for the specified packet type.
        /// </summary>
        /// <param name="forType">The packet type to select the handler for.</param>
        /// <returns>An instance of an <see cref="IHandler"/> implementaion.</returns>
        public IHandler SelectForType(byte forType)
        {
            IHandler handler = this.handlersKnown.Single(h => h.ForPacketType == forType);

            if (handler == null)
            {
                handler = new DefaultHandler(this.logger, forType);
            }

            return handler;
        }
    }
}
