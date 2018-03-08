// <copyright file="LocationsMatchPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Server.Data.Models.Structs;

    internal class LocationsMatchPolicy : LocationsAreDistantByPolicy
    {
        public LocationsMatchPolicy(Location locationOne, Location locationTwo, byte distance = 0, bool sameFloorOnly = true)
            : base(locationOne, locationTwo, distance, sameFloorOnly)
        {
            // Nothing else...
        }
    }
}