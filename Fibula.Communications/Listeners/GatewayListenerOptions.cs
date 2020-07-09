// -----------------------------------------------------------------
// <copyright file="GatewayListenerOptions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
