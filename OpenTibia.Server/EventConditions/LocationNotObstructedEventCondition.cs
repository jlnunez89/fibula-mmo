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

namespace OpenTibia.Server.EventConditions
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling.Contracts.Abstractions;
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
        /// <param name="tileAccessor">A reference to the tile accessor in use.</param>
        /// <param name="requestingCreature">The creature requesting the event.</param>
        /// <param name="determineThingMovingFunc">A delegate function to determine the thing that is being moved.</param>
        /// <param name="determineTargetLocationFunc">A function to determine the target location to check.</param>
        public LocationNotObstructedEventCondition(ITileAccessor tileAccessor, ICreature requestingCreature, Func<IThing> determineThingMovingFunc, Func<Location> determineTargetLocationFunc)
        {
            tileAccessor.ThrowIfNull(nameof(tileAccessor));
            determineThingMovingFunc.ThrowIfNull(nameof(determineThingMovingFunc));
            determineTargetLocationFunc.ThrowIfNull(nameof(determineTargetLocationFunc));

            this.TileAccessor = tileAccessor;

            this.Requestor = requestingCreature;
            this.GetThingMoving = determineThingMovingFunc;
            this.GetLocation = determineTargetLocationFunc;
        }

        /// <summary>
        /// Gets the creature requesting the event.
        /// </summary>
        public ICreature Requestor { get; }

        /// <summary>
        /// Gets the reference to the tile accessor.
        /// </summary>
        public ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Gets the delegate function to determine the <see cref="IThing"/> being moved.
        /// </summary>
        public Func<IThing> GetThingMoving { get; }

        /// <summary>
        /// Gets the delegate function to determine the location being checked.
        /// </summary>
        public Func<Location> GetLocation { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "There is not enough room.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var thingMoving = this.GetThingMoving();

            if (this.Requestor == null || thingMoving == null || !this.TileAccessor.GetTileAt(this.GetLocation(), out ITile destinationTile))
            {
                // requestor being null means this was probably called from a script.
                // Not this policy's job to restrict this.
                return true;
            }

            // creature trying to land on a blocking item.
            if (thingMoving is ICreature && destinationTile.BlocksPass)
            {
                return false;
            }

            if (thingMoving is IItem thingAsItem)
            {
                if (destinationTile.BlocksLay || (thingAsItem.BlocksPass && destinationTile.BlocksPass))
                {
                    return false;
                }
            }

            return true;
        }
    }
}