// -----------------------------------------------------------------
// <copyright file="GatewayLogInPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Incoming
{
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a login packet routed to the gateway server.
    /// </summary>
    public sealed class GatewayLogInPacket : IGatewayLoginInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayLogInPacket"/> class.
        /// </summary>
        /// <param name="clientVersion">The version of the client attempting to connect.</param>
        /// <param name="clientOs">The operating system of the client attempting to connect.</param>
        /// <param name="xteaKey">The values for the xtea key.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="password">The password observed.</param>
        public GatewayLogInPacket(ushort clientVersion, ushort clientOs, uint[] xteaKey, uint accountNumber, string password)
        {
            this.ClientVersion = clientVersion;
            this.ClientOs = clientOs;
            this.XteaKey = xteaKey;
            this.AccountNumber = accountNumber;
            this.Password = password;
        }

        /// <summary>
        /// Gets the account number.
        /// </summary>
        public uint AccountNumber { get; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Gets the xtea key in use.
        /// </summary>
        public uint[] XteaKey { get; }

        /// <summary>
        /// Gets the version of the client attempting to connect.
        /// </summary>
        public int ClientVersion { get; }

        /// <summary>
        /// Gets the operating system of the client attempting to connect.
        /// </summary>
        public ushort ClientOs { get; }
    }
}
