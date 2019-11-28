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

namespace OpenTibia.Server.MovementEvents.EventConditions
{
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents an event condition that evaluates whether two locations match.
    /// </summary>
    internal class LocationsMatchEventCondition : LocationsAreDistantByEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsMatchEventCondition"/> class.
        /// </summary>
        /// <param name="locationOne">The first location.</param>
        /// <param name="locationTwo">The second location.</param>
        public LocationsMatchEventCondition(Location locationOne, Location locationTwo)
            : base(locationOne, locationTwo, 0, true)
        {
            // Nothing else...
        }
    }
}