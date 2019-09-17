// -----------------------------------------------------------------
// <copyright file="ProtocolFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Options;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that provides methods for <see cref="IProtocol"/> creation.
    /// </summary>
    public class ProtocolFactory : IProtocolFactory
    {
        /// <summary>
        /// The game configuration options.
        /// </summary>
        private readonly GameConfigurationOptions gameConfig;

        /// <summary>
        /// The protocol configuration options.
        /// </summary>
        private readonly ProtocolConfigurationOptions protocolConfig;

        /// <summary>
        /// Holds the handler selectors known to this factory, injected by dependency injection, and passed down to the protocol instance.
        /// </summary>
        private readonly IList<IHandlerSelector> handlerSelectorsKnown;

        /// <summary>
        /// Holds the protocol singletons that have been created, by type.
        /// </summary>
        private readonly IDictionary<OpenTibiaProtocolType, IProtocol> protocolInstancesCreated;

        /// <summary>
        /// Lock to semaphore protocol singleton creation.
        /// </summary>
        private readonly object protocolCreationLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolFactory"/> class.
        /// </summary>
        /// <param name="handlerSelectors">The handler selectors to pass down to the protocol instance.</param>
        /// <param name="gameConfigOptions">A reference to the game configuration options.</param>
        /// <param name="protocolConfigOptions">A referebce to the protocol configuration options.</param>
        public ProtocolFactory(
            IEnumerable<IHandlerSelector> handlerSelectors,
            IOptions<GameConfigurationOptions> gameConfigOptions,
            IOptions<ProtocolConfigurationOptions> protocolConfigOptions)
        {
            this.handlerSelectorsKnown = handlerSelectors.ToList();
            this.gameConfig = gameConfigOptions.Value;
            this.protocolConfig = protocolConfigOptions.Value;

            this.protocolInstancesCreated = new Dictionary<OpenTibiaProtocolType, IProtocol>();
            this.protocolCreationLock = new object();
        }

        /// <summary>
        /// Creates an instance of an implementation of <see cref="IProtocol"/> depending on the provided type.
        /// </summary>
        /// <param name="protocolType">The type of protocol to instantiate.</param>
        /// <returns>A new <see cref="IProtocol"/> implementation instance.</returns>
        public IProtocol CreateForType(OpenTibiaProtocolType protocolType)
        {
            if (!this.protocolInstancesCreated.ContainsKey(protocolType))
            {
                lock (this.protocolCreationLock)
                {
                    if (!this.protocolInstancesCreated.ContainsKey(protocolType))
                    {
                        var handlerSelector = this.handlerSelectorsKnown.FirstOrDefault(handler => handler.ForProtocol == protocolType);

                        if (handlerSelector == null)
                        {
                            throw new NotSupportedException($"There was no {nameof(IHandlerSelector)} registered for protocol type '{protocolType}'.");
                        }

                        IProtocol protocolToAdd = null;

                        switch (protocolType)
                        {
                            case OpenTibiaProtocolType.LoginProtocol:
                                protocolToAdd = new LoginProtocol(handlerSelector, this.protocolConfig, this.gameConfig);

                                break;
                            case OpenTibiaProtocolType.GameProtocol:
                                protocolToAdd = new GameProtocol(handlerSelector, this.protocolConfig);

                                break;
                            case OpenTibiaProtocolType.ManagementProtocol:
                                protocolToAdd = new ManagementProtocol(handlerSelector);

                                break;
                            default:
                                throw new NotSupportedException($"The protocol type '{protocolType}' is not supported by this factory.");
                        }

                        this.protocolInstancesCreated.Add(protocolType, protocolToAdd);
                    }
                }
            }

            return this.protocolInstancesCreated[protocolType];
        }
    }
}
