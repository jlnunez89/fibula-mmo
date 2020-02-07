// -----------------------------------------------------------------
// <copyright file="TileContainsThingEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Conditions
{
    using System;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents an event condition that evaluates whether a tile in a location contains the specified thing.
    /// </summary>
    public class TileContainsThingEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileContainsThingEventCondition"/> class.
        /// </summary>
        /// <param name="tileAccessor">A reference to the tile accessor.</param>
        /// <param name="thing">The thing to check for.</param>
        /// <param name="location">The location to check at.</param>
        /// <param name="count">Optional. The amount to check for. Default is 1.</param>
        public TileContainsThingEventCondition(ITileAccessor tileAccessor, IThing thing, Location location, byte count = 1)
        {
            if (count == 0 || count > 100)
            {
                throw new ArgumentException($"Invalid count {count}.", nameof(count));
            }

            this.TileAccessor = tileAccessor;

            this.Thing = thing;
            this.Count = count;
            this.Location = location;
        }

        /// <summary>
        /// Gets the location to check.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the reference to the tile accessor.
        /// </summary>
        public ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Gets the <see cref="IThing"/> to check.
        /// </summary>
        public IThing Thing { get; }

        /// <summary>
        /// Gets the amount to check for.
        /// </summary>
        public byte Count { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "Sorry, not possible.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            if (this.Thing == null)
            {
                return false;
            }

            if (!this.TileAccessor.GetTileAt(this.Location, out ITile sourceTile) || sourceTile.GetStackPositionOfThing(this.Thing) == byte.MaxValue)
            {
                // This tile no longer has the thing, or it's obstructed (i.e. someone placed something on top of it).
                return false;
            }

            return true;
        }
    }
}