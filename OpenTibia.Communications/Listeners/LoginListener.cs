// -----------------------------------------------------------------
// <copyright file="LoginListener.cs" company="2Dudes">
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
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Security.Contracts;

    /// <summary>
    /// Class that extends the standard <see cref="BaseListener"/> for the login protocol.
    /// </summary>
    public class LoginListener : BaseListener
    {
        /// <summary>
        /// The default port which the login listener will be listening to.
        /// </summary>
        private const int DefaultLoginListenerPort = 7171;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginListener"/> class.
        /// </summary>
        /// <param name="protocolFactory">The protocol factory reference to create a protocol that the listerner will follow.</param>
        /// <param name="dosDefender">A reference to the DoS defender service implementation.</param>
        /// <param name="connectionManager">A reference to the connection manager service implementation.</param>
        /// <param name="port">The port where this listener will listen.</param>
        public LoginListener(IProtocolFactory protocolFactory, IDoSDefender dosDefender, IConnectionManager connectionManager, int port = DefaultLoginListenerPort)
            : base(port, protocolFactory?.CreateForType(OpenTibiaProtocolType.LoginProtocol), dosDefender, connectionManager)
        {
        }
    }
}
