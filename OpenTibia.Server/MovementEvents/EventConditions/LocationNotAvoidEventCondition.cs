// -----------------------------------------------------------------
// <copyright file="LocationNotAvoidEventCondition.cs" company="2Dudes">
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
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents an event condition that evaluates whether a location does not have a tile with an avoid flag set.
    /// </summary>
    internal class LocationNotAvoidEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationNotAvoidEventCondition"/> class.
        /// </summary>
        /// <param name="tileAccessor">A reference to the tile accessor in use.</param>
        /// <param name="requestingCreature">The creature requesting the move.</param>
        /// <param name="determineThingMovingFunc">A delegate function to determine the thing that is being moved.</param>
        /// <param name="determineLocationFunc">A delegate function to determine the location to check.</param>
        public LocationNotAvoidEventCondition(ITileAccessor tileAccessor, ICreature requestingCreature, Func<IThing> determineThingMovingFunc, Func<Location> determineLocationFunc)
        {
            tileAccessor.ThrowIfNull(nameof(tileAccessor));
            determineThingMovingFunc.ThrowIfNull(nameof(determineThingMovingFunc));
            determineLocationFunc.ThrowIfNull(nameof(determineLocationFunc));

            this.TileAccessor = tileAccessor;
            this.Requestor = requestingCreature;
            this.GetThing = determineThingMovingFunc;
            this.GetLocation = determineLocationFunc;
        }

        /// <summary>
        /// Gets the creature requesting the move.
        /// </summary>
        public ICreature Requestor { get; }

        /// <summary>
        /// Gets the reference to the tile accessor.
        /// </summary>
        public ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Gets a delegate function to determine the <see cref="IThing"/> being moved.
        /// </summary>
        public Func<IThing> GetThing { get; }

        /// <summary>
        /// Gets a delegate function to determine the location being checked.
        /// </summary>
        public Func<Location> GetLocation { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "Sorry, not possible.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var thing = this.GetThing();

            if (this.Requestor == null || thing == null)
            {
                // requestor being null means this was probably called from a script.
                // Not this policy's job to restrict this.
                return true;
            }

            return !(thing is ICreature creature) || this.Requestor == creature /*|| (this.TileAccessor.GetTileAt(this.GetLocation(), out ITile destTile) && destTile.CanBeWalked()) */;
        }
    }
}