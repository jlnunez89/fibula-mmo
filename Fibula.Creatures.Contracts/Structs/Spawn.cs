// -----------------------------------------------------------------
// <copyright file="Spawn.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Structs
{
    using System;
    using Fibula.Common.Contracts.Structs;

    /// <summary>
    /// Structure that represents spawns.
    /// </summary>
    public struct Spawn
    {
        /// <summary>
        /// Gets or sets the race id of monsters in this spawn.
        /// </summary>
        public ushort MonsterRaceId { get; set; }

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
