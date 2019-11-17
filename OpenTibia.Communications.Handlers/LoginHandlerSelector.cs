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

namespace OpenTibia.Communications.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a handler selector for incoming login requests.
    /// </summary>
    public class LoginHandlerSelector : IHandlerSelector
    {
        /// <summary>
        /// The known handlers to pick from.
        /// </summary>
        private readonly List<IHandler> handlersKnown;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginHandlerSelector"/> class.
        /// </summary>
        /// <param name="handlersRegistered">The collection of handlers registered in the configuration root of the service.</param>
        public LoginHandlerSelector(IEnumerable<IHandler> handlersRegistered)
        {
            handlersRegistered.ThrowIfNull(nameof(handlersRegistered));

            this.handlersKnown = handlersRegistered.ToList();
        }

        /// <summary>
        /// Gets the protocol type for which this handler selector works.
        /// </summary>
        public OpenTibiaProtocolType ForProtocol => OpenTibiaProtocolType.LoginProtocol;

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
                handler = new DefaultHandler(forType);
            }

            return handler;
        }
    }
}
