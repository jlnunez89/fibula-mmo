// -----------------------------------------------------------------
// <copyright file="AddressBindingOptions.cs" company="2Dudes">
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
    /// Class that represents binding options for an IPv4 address and port.
    /// </summary>
    public class AddressBindingOptions
    {
        /// <summary>
        /// Gets or sets the IPv4 address to bind to.
        /// </summary>
        public string Ipv4Address { get; set; }

        /// <summary>
        /// Gets or sets the port to bind to.
        /// </summary>
        public ushort Port { get; set; }
    }
}