// -----------------------------------------------------------------
// <copyright file="GatewayLogInPacket.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Incoming
{
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a login packet routed to the gateway server.
    /// </summary>
    public sealed class GatewayLogInPacket : IIncomingPacket, IGatewayLoginInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayLogInPacket"/> class.
        /// </summary>
        /// <param name="clientVersion">The version of the client attempting to connect.</param>
        /// <param name="clientOs">The operating system of the client attempting to connect.</param>
        /// <param name="xteaKey">The values for the xtea key.</param>
        /// <param name="accountName">The account name.</param>
        /// <param name="password">The password observed.</param>
        public GatewayLogInPacket(ushort clientVersion, ushort clientOs, uint[] xteaKey, string accountName, string password)
        {
            this.ClientVersion = clientVersion;
            this.ClientOs = clientOs;
            this.XteaKey = xteaKey;
            this.AccountName = accountName;
            this.Password = password;
        }

        /// <summary>
        /// Gets the account name.
        /// </summary>
        public string AccountName { get; }

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
