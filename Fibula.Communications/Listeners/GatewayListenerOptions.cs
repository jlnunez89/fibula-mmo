// -----------------------------------------------------------------
// <copyright file="GatewayListenerOptions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Listeners
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class that represents options for the gateway listener.
    /// </summary>
    public class GatewayListenerOptions : BaseListenerOptions
    {
        /// <summary>
        /// Gets or sets the port to listen to.
        /// </summary>
        [Required(ErrorMessage = "A port for the gateway listener must be speficied.")]
        public override ushort? Port { get; set; }
    }
}
