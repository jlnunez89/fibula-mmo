// -----------------------------------------------------------------
// <copyright file="LocationNotAviodEventCondition.cs" company="2Dudes">
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
    /// Class that represents an event condition that evaluates whether a location does not have a tile with an avoid flag set.
    /// </summary>
    internal class LocationNotAviodEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationNotAviodEventCondition"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        /// <param name="tileAccessor">A reference to the tile accessor in use.</param>
        /// <param name="requestingCreatureId">The id of the requesting creature.</param>
        /// <param name="thing">The thing being moved.</param>
        /// <param name="location">The location being checked.</param>
        public LocationNotAviodEventCondition(ICreatureFinder creatureFinder, ITileAccessor tileAccessor, uint requestingCreatureId, IThing thing, Location location)
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
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the reference to the tile accessor.
        /// </summary>
        public ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Gets the location being checked.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the <see cref="IThing"/> being moved.
        /// </summary>
        public IThing Thing { get; }

        /// <summary>
        /// Gets the id of the creature that requested the move.
        /// </summary>
        public uint RequestorId { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "Sorry, not possible.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var requestor = this.RequestorId == 0 ? null : this.CreatureFinder.FindCreatureById(this.RequestorId);

            this.TileAccessor.GetTileAt(this.Location, out ITile destTile);

            if (requestor == null || this.Thing == null)
            {
                // requestor being null means this was probably called from a script.
                // Not this policy's job to restrict this.
                return true;
            }

            return !(this.Thing is ICreature) || requestor == this.Thing /*|| destTile.CanBeWalked()*/;
        }
    }
}