// -----------------------------------------------------------------
// <copyright file="ProtocolConfigurationOptions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts
{
    /// <summary>
    /// Class that represents options for configuration of a protocol.
    /// </summary>
    public class ProtocolConfigurationOptions
    {
        /// <summary>
        /// Gets or sets the server version.
        /// </summary>
        public ProtocolVersion ServerVersion { get; set; }

        /// <summary>
        /// Gets or sets the client version.
        /// </summary>
        public ProtocolVersion ClientVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the protocol is using CipSoft RSA keys for encryption.
        /// </summary>
        // TODO: move to somewhere else.
        public bool UsingCipsoftRsaKeys { get; set; }

        /// <summary>
        /// Gets or sets the query manager's password.
        /// </summary>
        // TODO: remove?
        public string QueryManagerPassword { get; set; }
    }
}
