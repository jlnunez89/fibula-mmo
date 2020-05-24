// -----------------------------------------------------------------
// <copyright file="GameListenerOptions.cs" company="2Dudes">
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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using OpenTibia.Communications.Contracts;

    /// <summary>
    /// Class that represents options for the game listener.
    /// </summary>
    public class GameListenerOptions : BaseListenerOptions
    {
        /// <summary>
        /// Gets or sets the client version.
        /// </summary>
        [Required(ErrorMessage = "A client version for the game listener must be speficied.")]
        public ProtocolVersion ClientVersion { get; set; }

        /// <summary>
        /// Gets or sets the IP address to listen to.
        /// </summary>
        [Required(ErrorMessage = "A public IP address for the game listener must be speficied.")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the port to listen to.
        /// </summary>
        [Required(ErrorMessage = "A port for the game listener must be speficied.")]
        public override ushort Port { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the protocol is using CipSoft RSA keys for encryption.
        /// </summary>
        // TODO: move to somewhere else.
        [DefaultValue(false)]
        public bool UsingCipsoftRsaKeys { get; set; }
    }
}
