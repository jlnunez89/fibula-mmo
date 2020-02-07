// -----------------------------------------------------------------
// <copyright file="LocationsAreDistantByEventCondition.cs" company="2Dudes">
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
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents an event condition that evaluates whether two locations are distant by at least a value, and if they should be on the same floor.
    /// </summary>
    public class LocationsAreDistantByEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsAreDistantByEventCondition"/> class.
        /// </summary>
        /// <param name="determineFirstLocationFunc">A function delegate to determine the first location.</param>
        /// <param name="determineSecondLocationFunc">A function delegate to determine the second location.</param>
        /// <param name="distance">Optional. The minimum distance that the loations must be distant by. Default is 1.</param>
        /// <param name="sameFloorOnly">Optional. Whether or not the locations must be on the same floor. Default is false.</param>
        public LocationsAreDistantByEventCondition(Func<Location> determineFirstLocationFunc, Func<Location> determineSecondLocationFunc, byte distance = 1, bool sameFloorOnly = false)
        {
            determineFirstLocationFunc.ThrowIfNull(nameof(determineFirstLocationFunc));
            determineSecondLocationFunc.ThrowIfNull(nameof(determineSecondLocationFunc));

            this.GetFirstLocation = determineFirstLocationFunc;
            this.GetSecondLocation = determineSecondLocationFunc;

            this.Distance = distance;
            this.SameFloorOnly = sameFloorOnly;
        }

        /// <summary>
        /// Gets the function to determine the first location.
        /// </summary>
        public Func<Location> GetFirstLocation { get; }

        /// <summary>
        /// Gets the function to determine the second location.
        /// </summary>
        public Func<Location> GetSecondLocation { get; }

        /// <summary>
        /// Gets the distance.
        /// </summary>
        public byte Distance { get; }

        /// <summary>
        /// Gets a value indicating whether locations on the same floor only are allowed.
        /// </summary>
        public bool SameFloorOnly { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "The destination is too far away.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var locationDiff = this.GetFirstLocation() - this.GetSecondLocation();
            var sameFloor = locationDiff.Z == 0;

            if (locationDiff.MaxValueIn2D <= this.Distance && (!this.SameFloorOnly || sameFloor))
            {
                // The thing is no longer in this position.
                return true;
            }

            return false;
        }
    }
}