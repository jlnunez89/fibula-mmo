// -----------------------------------------------------------------
// <copyright file="IGatewayLoginInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Contracts.Abstractions
{
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Interface for the login information supplied on a gateway server login request.
    /// </summary>
    public interface IGatewayLoginInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the account number.
        /// </summary>
        uint AccountNumber { get; }

        /// <summary>
        /// Gets the account password.
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Gets the XTea encryption bytes.
        /// </summary>
        uint[] XteaKey { get; }

        /// <summary>
        /// Gets the version of the client attempting to connect.
        /// </summary>
        int ClientVersion { get; }

        /// <summary>
        /// Gets the operating system of the client attempting to connect.
        /// </summary>
        ushort ClientOs { get; }
    }
}
