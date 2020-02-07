// -----------------------------------------------------------------
// <copyright file="LocationsMatchEventCondition.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents an event condition that evaluates whether two locations match.
    /// </summary>
    public class LocationsMatchEventCondition : LocationsAreDistantByEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsMatchEventCondition"/> class.
        /// </summary>
        /// <param name="determineFirstLocationFunc">A function delegate to determine the first location.</param>
        /// <param name="determineSecondLocationFunc">A function delegate to determine the second location.</param>
        public LocationsMatchEventCondition(Func<Location> determineFirstLocationFunc, Func<Location> determineSecondLocationFunc)
            : base(determineFirstLocationFunc, determineSecondLocationFunc, distance: 0, sameFloorOnly: true)
        {
            // Nothing else...
        }
    }
}