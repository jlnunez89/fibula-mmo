// -----------------------------------------------------------------
// <copyright file="LocationNotObstructedEventCondition.cs" company="2Dudes">
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
    /// Class that represents an event condition that evaluates whether a location is not obstructed.
    /// </summary>
    internal class LocationNotObstructedEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationNotObstructedEventCondition"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        /// <param name="tileAccessor">A reference to the tile accessor in use.</param>
        /// <param name="requestingCreatureId">The id of the creature requesting the event.</param>
        /// <param name="thing">The thing being moved in the event.</param>
        /// <param name="location">The location being checked.</param>
        public LocationNotObstructedEventCondition(ICreatureFinder creatureFinder, ITileAccessor tileAccessor, uint requestingCreatureId, IThing thing, Location location)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));
            tileAccessor.ThrowIfNull(nameof(tileAccessor));

            this.CreatureFinder = creatureFinder;
            this.TileAccessor = tileAccessor;

            this.RequestorId = requestingCreatureId;
            this.Thing = thing;
            this.Location = location;
        }

        /// <summary>
        /// Gets the location being checked.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the reference to the tile accessor.
        /// </summary>
        public ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Gets the <see cref="IThing"/> being moved.
        /// </summary>
        public IThing Thing { get; }

        /// <summary>
        /// Gets the id of the creature requesting the event.
        /// </summary>
        public uint RequestorId { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "There is not enough room.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var requestor = this.RequestorId == 0 ? null : this.CreatureFinder.FindCreatureById(this.RequestorId);

            this.TileAccessor.GetTileAt(this.Location, out Tile destTile);

            if (requestor == null || this.Thing == null)
            {
                // requestor being null means this was probably called from a script.
                // Not this policy's job to restrict this.
                return true;
            }

            //// creature trying to land on a blocking item.
            //if (destTile.BlocksPass && this.Thing is ICreature)
            //{
            //    return false;
            //}

            //if (this.Thing is IItem thingAsItem)
            //{
            //    if (destTile.BlocksLay || (thingAsItem.BlocksPass && destTile.BlocksPass))
            //    {
            //        return false;
            //    }
            //}

            return true;
        }
    }
}