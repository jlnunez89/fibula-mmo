// -----------------------------------------------------------------
// <copyright file="WorldConfigurationOptions.cs" company="2Dudes">
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
    using OpenTibia.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents options for the configuration of a world.
    /// </summary>
    public class WorldConfigurationOptions
    {
        /// <summary>
        /// Gets or sets the world type.
        /// </summary>
        public WorldType Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the world.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the website url.
        /// </summary>
        // TODO: should this really be per-world?
        public string WebsiteUrl { get; set; }

        /// <summary>
        /// Gets or sets the geolocation of this world server.
        /// </summary>
        public string Geolocation { get; set; }

        /// <summary>
        /// Gets or sets the hour at which the world reset takes place in local time.
        /// </summary>
        public byte LocalResetHour { get; set; }

        /// <summary>
        /// Gets or sets the world's message of the day.
        /// </summary>
        // TODO: move this to control plane.
        public string MessageOfTheDay { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of veteran player characters allowed to log online simultaneously.
        /// </summary>
        public ushort MaximumOnlineVeterans { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of newbie player characters allowed to log online simultaneously.
        /// </summary>
        public ushort MaximumOnlineNewbies { get; set; }

        /// <summary>
        /// Gets or sets the maximum queue size for veteran player characters.
        /// </summary>
        public ushort MaximumVeteranQueueSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum queue size for newbie player characters.
        /// </summary>
        public ushort MaximumNewbieQueueSize { get; set; }
    }
}
