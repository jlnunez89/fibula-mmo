﻿// -----------------------------------------------------------------
// <copyright file="ApplicationContextOptions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class that represents options for the application context.
    /// </summary>
    /// <remarks>
    /// Settings marked with [MoveToControlPlane] should ultimately be moved to a dynamic control plane.
    /// </remarks>
    public class ApplicationContextOptions
    {
        /// <summary>
        /// Gets or sets the supported client version information.
        /// </summary>
        [Required(ErrorMessage = "A supported client version must be speficied.")]
        public VersionInformation SupportedClientVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the protocol is using CipSoft RSA keys for encryption.
        /// </summary>
        [DefaultValue(false)]
        public bool UsingCipsoftRsaKeys { get; set; }

        /// <summary>
        /// Gets or sets the website url.
        /// </summary>
        public string WebsiteUrl { get; set; }

        /// <summary>
        /// Gets or sets the configuration of the game world.
        /// </summary>
        /// <remarks>
        /// [MoveToControlPlane].
        /// </remarks>
        [Required(ErrorMessage = "Settings for the world configuration must be speficied.")]
        public WorldConfiguration World { get; set; }
    }
}
