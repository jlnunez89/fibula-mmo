// -----------------------------------------------------------------
// <copyright file="LocationHasTileWithGroundEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.MovementEvents.EventConditions
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents an event condition that evaluates whether a location has a tile with ground on it.
    /// </summary>
    internal class LocationHasTileWithGroundEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationHasTileWithGroundEventCondition"/> class.
        /// </summary>
        /// <param name="tileAccessor">A reference to the tile accessor in use.</param>
        /// <param name="location">The location to check.</param>
        public LocationHasTileWithGroundEventCondition(ITileAccessor tileAccessor, Location location)
        {
            tileAccessor.ThrowIfNull(nameof(tileAccessor));

            this.TileAccessor = tileAccessor;
            this.Location = location;
        }

        /// <summary>
        /// Gets the reference to the tile accessor.
        /// </summary>
        public ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Gets the location to check.
        /// </summary>
        public Location Location { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "There is not enough room.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            return this.TileAccessor.GetTileAt(this.Location, out ITile tile) && tile.Ground != null;
        }
    }
}