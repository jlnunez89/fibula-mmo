// -----------------------------------------------------------------
// <copyright file="AccountLoginPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an account login packet.
    /// </summary>
    public sealed class AccountLoginPacket : IIncomingPacket, IAccountLoginInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountLoginPacket"/> class.
        /// </summary>
        /// <param name="xteaKey">The values for the xtea key.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="password">The password observed.</param>
        public AccountLoginPacket(uint[] xteaKey, uint accountNumber, string password)
        {
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
    }
}
