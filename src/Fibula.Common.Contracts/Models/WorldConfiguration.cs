// -----------------------------------------------------------------
// <copyright file="WorldConfiguration.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Models
{
    using System.ComponentModel.DataAnnotations;
    using Fibula.Common.Contracts.Enumerations;

    /// <summary>
    /// Class that represents options for the configuration of a world.
    /// </summary>
    public class WorldConfiguration
    {
        /// <summary>
        /// Gets or sets the world type.
        /// </summary>
        [Required(ErrorMessage = "A type of world must be speficied.")]
        public WorldType? Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the world.
        /// </summary>
        [Required(ErrorMessage = "A name for the world must be speficied.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the public IP Address of the world server.
        /// </summary>
        [Required(ErrorMessage = "A public IP address for the gameworld server must be speficied.")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the port of the world server.
        /// </summary>
        [Required(ErrorMessage = "A port for the gameworld server must be speficied.")]
        public ushort? Port { get; set; }

        /// <summary>
        /// Gets or sets the geolocation of this world server.
        /// </summary>
        [Required(ErrorMessage = "A geolocation for the world must be speficied.")]
        public string Geolocation { get; set; }

        /// <summary>
        /// Gets or sets the hour at which the world reset takes place in local time.
        /// Informational only.
        /// </summary>
        public byte LocalResetHour { get; set; }

        /// <summary>
        /// Gets or sets the world's message of the day.
        /// </summary>
        /// <remarks>
        /// Should eventually come from a dynamic control plane framework.
        /// </remarks>
        public string MessageOfTheDay { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of veteran player characters allowed to log online simultaneously.
        /// </summary>
        // [Required(ErrorMessage = "A value for the maximum number of veterans simultaneously logged into the world must be speficied.")]
        public ushort? MaximumOnlineVeterans { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of newbie player characters allowed to log online simultaneously.
        /// </summary>
        // [Required(ErrorMessage = "A value for the maximum number of newbies simultaneously logged into the world must be speficied.")]
        public ushort? MaximumOnlineNewbies { get; set; }

        /// <summary>
        /// Gets or sets the maximum queue size for veteran player characters.
        /// </summary>
        // [Required(ErrorMessage = "A value for the maximum number of veterans waiting in the login queue to the world must be speficied.")]
        public ushort? MaximumVeteranQueueSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum queue size for newbie player characters.
        /// </summary>
        // [Required(ErrorMessage = "A value for the maximum number of newbies waiting in the login queue to the world must be speficied.")]
        public ushort? MaximumNewbieQueueSize { get; set; }
    }
}
