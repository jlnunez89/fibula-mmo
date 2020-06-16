// -----------------------------------------------------------------
// <copyright file="Spawn.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Contracts.Structs
{
    using System;

    /// <summary>
    /// Structure that represents spawns.
    /// </summary>
    public struct Spawn
    {
        /// <summary>
        /// Gets or sets the id of entities in this spawn.
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// Gets or sets the location of the spawn.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets the radius of the spawn.
        /// </summary>
        public ushort Radius { get; set; }

        /// <summary>
        /// Gets or sets the count of entities to spawn.
        /// </summary>
        public byte Count { get; set; }

        /// <summary>
        /// Gets or sets the regeneration time of this spawn.
        /// </summary>
        public TimeSpan Regen { get; set; }
    }
}
