// -----------------------------------------------------------------
// <copyright file="GameConfigurationOptions.cs" company="2Dudes">
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
    /// Class that represents options for game configuration.
    /// </summary>
    public class GameConfigurationOptions
    {
        /// <summary>
        /// Gets or sets the world configuration options.
        /// </summary>
        public WorldConfigurationOptions World { get; set; }

        /// <summary>
        /// Gets or sets the public address binding options.
        /// </summary>
        public AddressBindingOptions PublicAddressBinding { get; set; }

        /// <summary>
        /// Gets or sets the private address binding options.
        /// </summary>
        public AddressBindingOptions PrivateAddressBinding { get; set; }
    }
}
